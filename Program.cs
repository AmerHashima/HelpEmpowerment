using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Repositories;
using HelpEmpowermentApi.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using HelpEmpowermentApi.Payments.Application;
using HelpEmpowermentApi.Payments.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace HelpEmpowermentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = UploadLimits.MaxVideoUploadSizeBytes;
            });

            // Register DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                     builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        sqlOptions.CommandTimeout(120);
                        sqlOptions.MigrationsAssembly("HelpEmpowermentApi");
                    });

                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            // ✅ ADD JWT AUTHENTICATION
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddHttpClient();
            var telrEnabled = builder.Configuration.GetValue<bool>($"{TelrOptions.SectionName}:Enabled");
            builder.Services.AddOptions<TelrOptions>().Bind(builder.Configuration.GetSection(TelrOptions.SectionName)).Validate(o => !o.Enabled || !string.IsNullOrWhiteSpace(o.StoreId), "Telr StoreId is required").Validate(o => !o.Enabled || !string.IsNullOrWhiteSpace(o.AuthKey), "Telr AuthKey must come from a secret source").Validate(o => !o.Enabled || !string.IsNullOrWhiteSpace(o.WebhookSecret), "Telr WebhookSecret must come from a secret source").Validate(o => !o.Enabled || (o.FrontendResultUrl is not null && Uri.CheckHostName(o.FrontendResultUrl.Host) != UriHostNameType.Unknown && (o.FrontendResultUrl.Scheme == Uri.UriSchemeHttp || o.FrontendResultUrl.Scheme == Uri.UriSchemeHttps)), "A valid HTTP or HTTPS Telr frontend result URL is required").ValidateOnStart();
            builder.Services.Configure<PaymentReconciliationOptions>(builder.Configuration.GetSection("PaymentReconciliation"));
            builder.Services.AddHttpClient<ITelrPaymentService, TelrPaymentService>(c => c.Timeout = TimeSpan.FromSeconds(20));
            builder.Services.AddSingleton<IClock, SystemClock>();
            builder.Services.AddSingleton<ITelrWebhookValidator, TelrWebhookValidator>();
            builder.Services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
            builder.Services.AddScoped<IInvoicePaymentProcessor, InvoicePaymentProcessor>();
            if (telrEnabled) builder.Services.AddHostedService<PaymentReconciliationService>();
            builder.Services.AddRateLimiter(o => { o.AddFixedWindowLimiter("payments-create", x => { x.PermitLimit = 5; x.Window = TimeSpan.FromMinutes(1); x.QueueLimit = 0; }); o.AddFixedWindowLimiter("payments-status", x => { x.PermitLimit = 30; x.Window = TimeSpan.FromMinutes(1); x.QueueLimit = 0; }); });
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = UploadLimits.MaxVideoUploadSizeBytes;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            builder.Services.AddControllers();
            //builder.Services.AddOpenApi();

            // Register repositories
            RegisterRepositories(builder.Services);
            
            // Register services
            RegisterServices(builder.Services);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto |
                    ForwardedHeaders.XForwardedHost;

                // Required for dynamic reverse proxies such as ngrok where the proxy IP is not stable.
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });

            var app = builder.Build();

            app.UseExceptionHandler(errorApp => errorApp.Run(async context => { var error = context.Features.Get<IExceptionHandlerFeature>()?.Error; var problem = new ProblemDetails { Type = "https://httpstatuses.com/500", Title = "An unexpected error occurred", Status = 500, Detail = app.Environment.IsDevelopment() ? error?.Message : "The request could not be completed." }; problem.Extensions["traceId"] = context.TraceIdentifier; problem.Extensions["errorCode"] = "UNEXPECTED_ERROR"; context.Response.StatusCode = 500; await context.Response.WriteAsJsonAsync(problem); }));

            // Auto-migrate database

            app.UseForwardedHeaders();

            //app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpEmpowerment API V1");
            });

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseRateLimiter();

            // ✅ ADD Authentication & Authorization Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
               .ExcludeFromDescription();
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
            app.MapControllers();

            app.Run();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICoursesMasterExamRepository, CoursesMasterExamRepository>();
            services.AddScoped<ICourseQuestionRepository, CourseQuestionRepository>();
            services.AddScoped<ICourseAnswerRepository, CourseAnswerRepository>();
            services.AddScoped<IAppLookupHeaderRepository, AppLookupHeaderRepository>();
            services.AddScoped<IAppLookupDetailRepository, AppLookupDetailRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseFeatureRepository, CourseFeatureRepository>();
            services.AddScoped<ICourseOutlineRepository, CourseOutlineRepository>();
            services.AddScoped<ICourseContentRepository, CourseContentRepository>();
            services.AddScoped<ICourseVideoRepository, CourseVideoRepository>();
            services.AddScoped<ICourseVideoAttachmentRepository, CourseVideoAttachmentRepository>();
            services.AddScoped<IStudentExamRepository, StudentExamRepository>();
            services.AddScoped<IStudentExamQuestionRepository, StudentExamQuestionRepository>();
            services.AddScoped<IStudentExamQuestionAnswerRepository, StudentExamQuestionAnswerRepository>();
            services.AddScoped<ICourseLiveSessionRepository, CourseLiveSessionRepository>();
            services.AddScoped<ICourseLiveSessionStudentRepository, CourseLiveSessionStudentRepository>();
            services.AddScoped<ICourseInstructorRepository, CourseInstructorRepository>();
            services.AddScoped<ICourseTargetAudienceRepository, CourseTargetAudienceRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ILinkRepository, LinkRepository>();
            services.AddScoped<IRoleLinkRepository, RoleLinkRepository>();
            // ========================================
            // ✅ NEW REPOSITORIES
            // ========================================
            services.AddScoped<IStudentCourseRepository, StudentCourseRepository>();
            services.AddScoped<IStudentBasketRepository, StudentBasketRepository>();
            services.AddScoped<IServiceContactUsRepository, ServiceContactUsRepository>();
            services.AddScoped<ILiveCourseRepository, LiveCourseRepository>();
            services.AddScoped<ILiveWebinarRepository, LiveWebinarRepository>();
            services.AddScoped<IStudentCourseReservationRepository, StudentCourseReservationRepository>();
            services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
            services.AddScoped<IStudentDeviceRepository, StudentDeviceRepository>();
            services.AddScoped<ICourseServiceRepository, CourseServiceRepository>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ILinkService, LinkService>();
            services.AddScoped<IRoleLinkService, RoleLinkService>();

        }

        private static void RegisterServices(IServiceCollection services)
        {
            // ✅ ADD AUTH SERVICE
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICoursesMasterExamService, CoursesMasterExamService>();
            services.AddScoped<ICourseQuestionService, CourseQuestionService>();
            services.AddScoped<IAppLookupService, AppLookupService>();
            services.AddScoped<ICourseAnswerService, CourseAnswerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseFeatureService, CourseFeatureService>();
            services.AddScoped<ICourseOutlineService, CourseOutlineService>();
            services.AddScoped<ICourseContentService, CourseContentService>();
            services.AddScoped<ICourseVideoService, CourseVideoService>();
            services.AddScoped<ICourseVideoAttachmentService, CourseVideoAttachmentService>();
            services.AddScoped<IStudentExamService, StudentExamService>();
            services.AddScoped<IStudentExamQuestionService, StudentExamQuestionService>();
            services.AddScoped<ICourseLiveSessionService, CourseLiveSessionService>();
            services.AddScoped<ICourseLiveSessionStudentService, CourseLiveSessionStudentService>();
            services.AddScoped<ICourseInstructorService, CourseInstructorService>();
            services.AddScoped<ICourseTargetAudienceService, CourseTargetAudienceService>();
            // ========================================
            // ✅ NEW SERVICES
            // ========================================
            services.AddScoped<IStudentCourseService, StudentCourseService>();
            services.AddScoped<IStudentBasketService, StudentBasketService>();
            services.AddScoped<IServiceContactUsService, ServiceContactUsService>();
            services.AddScoped<ILiveCourseService, LiveCourseService>();
            services.AddScoped<ILiveWebinarService, LiveWebinarService>();
            services.AddScoped<IStudentCourseReservationService, StudentCourseReservationService>();
            services.AddScoped<IUserDeviceService, UserDeviceService>();
            services.AddScoped<ICourseServiceDetailService, CourseServiceDetailService>();
        }


    }
}
