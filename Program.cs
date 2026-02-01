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

            // Add Services to the container.

            // Register DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<ICoursesMasterExamRepository, CoursesMasterExamRepository>();
            builder.Services.AddScoped<ICourseQuestionRepository, CourseQuestionRepository>();
            builder.Services.AddScoped<ICourseAnswerRepository, CourseAnswerRepository>();
            builder.Services.AddScoped<IAppLookupHeaderRepository, AppLookupHeaderRepository>();
            builder.Services.AddScoped<IAppLookupDetailRepository, AppLookupDetailRepository>();

            // Services
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<ICoursesMasterExamService, CoursesMasterExamService>();
            builder.Services.AddScoped<ICourseQuestionService, CourseQuestionService>();
            builder.Services.AddScoped<IAppLookupService, AppLookupService>();
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
    }
}
