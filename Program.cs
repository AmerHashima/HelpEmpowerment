using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Repositories;
using HelpEmpowermentApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace HelpEmpowermentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            var app = builder.Build();

            // Auto-migrate database

            //app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpEmpowerment API V1");
            });

            app.UseHttpsRedirection();
            app.UseCors();

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
            // ========================================
            // ✅ NEW REPOSITORIES
            // ========================================
            services.AddScoped<IStudentCourseRepository, StudentCourseRepository>();
            services.AddScoped<IStudentBasketRepository, StudentBasketRepository>();
            services.AddScoped<IServiceContactUsRepository, ServiceContactUsRepository>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // ✅ ADD AUTH SERVICE
            services.AddScoped<IAuthService, AuthService>();
            
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
        }


    }
}
