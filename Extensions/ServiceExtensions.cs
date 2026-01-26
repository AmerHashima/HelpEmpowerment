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
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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