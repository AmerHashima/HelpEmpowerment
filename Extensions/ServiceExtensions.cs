using Microsoft.EntityFrameworkCore;
using StandardArticture.Data;
using StandardArticture.IRepositories;
using StandardArticture.IServices;
using StandardArticture.Repositories;
using StandardArticture.Services;

namespace StandardArticture.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database with aggressive retry logic for Docker/containerized environments
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
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
                        sqlOptions.MigrationsAssembly("StandardArticture");
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

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICoursesMasterExamRepository, CoursesMasterExamRepository>();
            services.AddScoped<ICourseQuestionRepository, CourseQuestionRepository>();
            services.AddScoped<ICourseAnswerRepository, CourseAnswerRepository>();
            services.AddScoped<IAppLookupHeaderRepository, AppLookupHeaderRepository>();
            services.AddScoped<IAppLookupDetailRepository, AppLookupDetailRepository>();

            // Services
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICoursesMasterExamService, CoursesMasterExamService>();
            services.AddScoped<ICourseQuestionService, CourseQuestionService>();
            services.AddScoped<IAppLookupService, AppLookupService>();

            return services;
        }
    }
}