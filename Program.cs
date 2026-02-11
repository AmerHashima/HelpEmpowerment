using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Repositories;
using HelpEmpowermentApi.Services;
using Microsoft.EntityFrameworkCore;

namespace HelpEmpowermentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Configure Google credentials FIRST
            ConfigureGoogleCredentials(builder.Environment);

            // Register DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                     builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        sqlOptions.CommandTimeout(120);
                        sqlOptions.MigrationsAssembly("HelpEmpowermentApi");
                    });

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Register all repositories and services
            RegisterRepositories(builder.Services);
            RegisterServices(builder.Services);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Auto-migrate database on startup
            MigrateDatabase(app);

            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpEmpowerment API V1");
            });

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthorization();
            
            // Health check endpoint
            app.MapGet("/health", () => Results.Ok(new 
            { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                googleCredentials = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"))
            })).ExcludeFromDescription();
            
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
            app.MapControllers();

            app.Run();
        }

        // ✅ Configure Google Credentials
        private static void ConfigureGoogleCredentials(IWebHostEnvironment environment)
        {
            try
            {
                const string credentialFileName = "test-erp-68be7-b83f4e97f6be.json";
                
                // Try multiple locations in order of priority
                var searchPaths = new[]
                {
                    // 1. Local development - from project Common folder
                    Path.Combine(environment.ContentRootPath, "Common", credentialFileName),
                    
                    // 2. Published app - from output Common folder
                    Path.Combine(AppContext.BaseDirectory, "Common", credentialFileName),
                    
                    // 3. Docker - copied to app root
                    Path.Combine(AppContext.BaseDirectory, credentialFileName),
                    
                    // 4. Fallback - current directory
                    Path.Combine(Directory.GetCurrentDirectory(), "Common", credentialFileName)
                };

                foreach (var path in searchPaths)
                {
                    if (File.Exists(path))
                    {
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                        Console.WriteLine($"✅ Google Cloud credentials loaded successfully");
                        Console.WriteLine($"   File: {credentialFileName}");
                        Console.WriteLine($"   Path: {path}");
                        Console.WriteLine($"   Environment: {environment.EnvironmentName}");
                        return;
                    }
                }

                // File not found - log warning but don't crash
                Console.WriteLine($"⚠️ Warning: Google credentials file '{credentialFileName}' not found");
                Console.WriteLine($"   Translation API will not be available");
                Console.WriteLine($"   Searched locations:");
                foreach (var path in searchPaths)
                {
                    Console.WriteLine($"     - {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error configuring Google credentials: {ex.Message}");
                Console.WriteLine($"   Translation API will not be available");
            }
        }

        // ✅ Auto-migrate database
        private static void MigrateDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                
                logger.LogInformation("🔄 Checking database migrations...");
                context.Database.Migrate();
                logger.LogInformation("✅ Database is up to date");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "❌ Database migration failed");
            }
        }

        // ✅ Register repositories
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
            services.AddScoped<ICourseLiveSessionRepository, CourseLiveSessionRepository>();
            services.AddScoped<ICourseLiveSessionStudentRepository, CourseLiveSessionStudentRepository>();
            services.AddScoped<ICourseInstructorRepository, CourseInstructorRepository>();
            services.AddScoped<ICourseTargetAudienceRepository, CourseTargetAudienceRepository>();
        }

        // ✅ Register services
        private static void RegisterServices(IServiceCollection services)
        {
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
        }
    }
}
