using Microsoft.EntityFrameworkCore;
using StandardArticture.Data;
using StandardArticture.Extensions;

namespace StandardArticture
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Application services (like DbContext, repositories, etc.)
            builder.Services.AddApplicationServices(builder.Configuration);

            // Response compression
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // CORS: allow all
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            // Routing, CORS & Authorization
            app.UseResponseCompression();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();

            // Map controllers
            app.MapControllers();

            // Run app
            app.Run();
        }
    }
}
