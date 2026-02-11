using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Repositories;
using HelpEmpowermentApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HelpEmpowermentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ IMPORTANT: Call this BEFORE building the app
            ConfigureGoogleCredentials(builder.Environment);

            // Add Services to the container.

            // ✅ FIX: Set Google credentials path with multiple fallback options
         //   SetGoogleCredentials(builder.Environment);

            // Register DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                     builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        // Enable retry on failure for transient errors (critical for Docker)
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,  // Increased for Docker startup delays
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);

                        // Command timeout (important for slow networks)
                        sqlOptions.CommandTimeout(120);

                        // Migrations assembly
                        sqlOptions.MigrationsAssembly("HelpEmpowermentApi");
                    });

                // Enable detailed errors in development
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }

                // Set query tracking behavior
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            
            builder.Services.AddHttpClient();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // ========================================
            // ✅ EXISTING REPOSITORIES
            // ========================================
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICoursesMasterExamRepository, CoursesMasterExamRepository>();
            builder.Services.AddScoped<ICourseQuestionRepository, CourseQuestionRepository>();
            builder.Services.AddScoped<ICourseAnswerRepository, CourseAnswerRepository>();
            builder.Services.AddScoped<IAppLookupHeaderRepository, AppLookupHeaderRepository>();
            builder.Services.AddScoped<IAppLookupDetailRepository, AppLookupDetailRepository>();

            // ========================================
            // ✅ NEW REPOSITORIES - AUTH & USERS
            // ========================================
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();

            // ========================================
            // ✅ NEW REPOSITORIES - COURSE FEATURES & CONTENT
            // ========================================
            builder.Services.AddScoped<ICourseFeatureRepository, CourseFeatureRepository>();
            builder.Services.AddScoped<ICourseOutlineRepository, CourseOutlineRepository>();
            builder.Services.AddScoped<ICourseContentRepository, CourseContentRepository>();
            builder.Services.AddScoped<ICourseVideoRepository, CourseVideoRepository>();
            builder.Services.AddScoped<ICourseVideoAttachmentRepository, CourseVideoAttachmentRepository>();

            // ========================================
            // ✅ NEW REPOSITORIES - STUDENT EXAMS
            // ========================================
            builder.Services.AddScoped<IStudentExamRepository, StudentExamRepository>();
            builder.Services.AddScoped<IStudentExamQuestionRepository, StudentExamQuestionRepository>();

            // ========================================
            // ✅ NEW REPOSITORIES - LIVE SESSIONS
            // ========================================
            builder.Services.AddScoped<ICourseLiveSessionRepository, CourseLiveSessionRepository>();
            builder.Services.AddScoped<ICourseLiveSessionStudentRepository, CourseLiveSessionStudentRepository>();

            // ========================================
            // ✅ NEW REPOSITORIES - INSTRUCTORS & TARGET AUDIENCE
            // ========================================
            builder.Services.AddScoped<ICourseInstructorRepository, CourseInstructorRepository>();
            builder.Services.AddScoped<ICourseTargetAudienceRepository, CourseTargetAudienceRepository>();

            // ========================================
            // ✅ EXISTING SERVICES
            // ========================================
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<ICoursesMasterExamService, CoursesMasterExamService>();
            builder.Services.AddScoped<ICourseQuestionService, CourseQuestionService>();
            builder.Services.AddScoped<IAppLookupService, AppLookupService>();
            builder.Services.AddScoped<ICourseAnswerService, CourseAnswerService>();

            // ========================================
            // ✅ NEW SERVICES - AUTH & USERS
            // ========================================
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IStudentService, StudentService>();

            // ========================================
            // ✅ NEW SERVICES - COURSE FEATURES & CONTENT
            // ========================================
            builder.Services.AddScoped<ICourseFeatureService, CourseFeatureService>();
            builder.Services.AddScoped<ICourseOutlineService, CourseOutlineService>();
            builder.Services.AddScoped<ICourseContentService, CourseContentService>();
            builder.Services.AddScoped<ICourseVideoService, CourseVideoService>();
            builder.Services.AddScoped<ICourseVideoAttachmentService, CourseVideoAttachmentService>();

            // ========================================
            // ✅ NEW SERVICES - STUDENT EXAMS
            // ========================================
            builder.Services.AddScoped<IStudentExamService, StudentExamService>();
            builder.Services.AddScoped<IStudentExamQuestionService, StudentExamQuestionService>();

            // ========================================
            // ✅ NEW SERVICES - LIVE SESSIONS
            // ========================================
            builder.Services.AddScoped<ICourseLiveSessionService, CourseLiveSessionService>();
            builder.Services.AddScoped<ICourseLiveSessionStudentService, CourseLiveSessionStudentService>();

            // ========================================
            // ✅ NEW SERVICES - INSTRUCTORS & TARGET AUDIENCE
            // ========================================
            builder.Services.AddScoped<ICourseInstructorService, CourseInstructorService>();
            builder.Services.AddScoped<ICourseTargetAudienceService, CourseTargetAudienceService>();

            // Add Swagger Services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS Services
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

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.MapOpenApi();

            // Enable Swagger middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpEmpowerment API V1");
            });
            // }

            app.UseHttpsRedirection();

            // Enable CORS
            app.UseCors();

            app.UseAuthorization();

            // Redirect root to Swagger
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

            app.MapControllers();

            app.Run();
        }

        // ✅ ADD THIS METHOD
        private static void SetGoogleCredentials(IWebHostEnvironment environment)
        {
            try
            {
                // Priority 1: Check environment variable (Docker/Production)
                var credentialsJson = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS_JSON");
                
                // Priority 2: Check appsettings.json (Local Development)
                if (string.IsNullOrEmpty(credentialsJson))
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();
                        
                    credentialsJson = configuration["GoogleCredentials:Json"];
                }
                
                if (!string.IsNullOrEmpty(credentialsJson))
                {
                    // Create temporary file
                    var tempPath = Path.Combine(Path.GetTempPath(), $"google-credentials-{Guid.NewGuid()}.json");
                    File.WriteAllText(tempPath, credentialsJson);
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", tempPath);
                    Console.WriteLine($"✅ Google credentials configured successfully");
                    Console.WriteLine($"   Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}");
                    Console.WriteLine($"   Temp file: {tempPath}");
                    return;
                }

                Console.WriteLine("⚠️ Warning: Google credentials not configured.");
                Console.WriteLine("   Translation API endpoints will not work.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error setting Google credentials: {ex.Message}");
            }
        }

        private static void ConfigureGoogleCredentials(IWebHostEnvironment environment)
        {
            const string CREDENTIAL_FILE = "test-erp-68be7-b83f4e97f6be.json";

            try
            {
                // Define search paths in priority order
                var searchPaths = new[]
                {
                    // Path 1: Common folder in content root (Local Development)
                    Path.Combine(environment.ContentRootPath, "Common", CREDENTIAL_FILE),
                    
                    // Path 2: Common folder in base directory (Published App)
                    Path.Combine(AppContext.BaseDirectory, "Common", CREDENTIAL_FILE),
                    
                    // Path 3: Base directory root (Docker fallback)
                    Path.Combine(AppContext.BaseDirectory, CREDENTIAL_FILE),
                    
                    // Path 4: Current directory
                    Path.Combine(Directory.GetCurrentDirectory(), "Common", CREDENTIAL_FILE)
                };

                // Try to find credentials file
                foreach (var path in searchPaths)
                {
                    if (File.Exists(path))
                    {
                        // Set environment variable pointing to file
                        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

                        Console.WriteLine("✅ Google Cloud Translation API configured");
                        Console.WriteLine($"   Environment: {environment.EnvironmentName}");
                        Console.WriteLine($"   Credentials: {path}");
                        return;
                    }
                }

                // Credentials not found
                Console.WriteLine("⚠️  Google Cloud credentials not found");
                Console.WriteLine($"   Translation API will be unavailable");
                Console.WriteLine($"   Searched paths:");
                foreach (var path in searchPaths)
                {
                    Console.WriteLine($"     - {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading Google credentials: {ex.Message}");
            }
        }
    }
}
