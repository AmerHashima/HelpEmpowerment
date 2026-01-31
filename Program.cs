using Microsoft.EntityFrameworkCore;
using StandardArticture.Data;
using StandardArticture.Extensions;
using Polly;

namespace StandardArticture
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --------------------------
            builder.Services.AddControllers();

            // Add Application & Infrastructure layers
            //builder.Services.AddApplication();
            //builder.Services.AddInfrastructure(builder.Configuration);

            //// --------------------------
            //// Configure FluentValidation
            //// --------------------------
            //builder.Services.AddFluentValidationAutoValidation()
            //                .AddFluentValidationClientsideAdapters();

            // --------------------------
            // Configure CORS to allow any region
            // --------------------------
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

            // Apply database migrations with retry logic (critical for Docker)
            await ApplyDatabaseMigrationsAsync(app);

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Map health check endpoints
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            app.MapGet("/", () => Results.Redirect("/swagger"));

            // ⭐ CORS must be before Authentication and Authorization
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            // Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();

        }

        private static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                logger.LogInformation("Starting database migration...");

                // Retry logic for database migration (important for Docker startup)
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        retryCount: 10,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (exception, timeSpan, retry, ctx) =>
                        {
                            logger.LogWarning(exception,
                                "Database migration attempt {Retry} failed. Waiting {TimeSpan} before next retry. Error: {Message}",
                                retry, timeSpan, exception.Message);
                        });

                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Test connection
                    await context.Database.CanConnectAsync();

                    // Apply migrations
                    await context.Database.MigrateAsync();

                    logger.LogInformation("Database migration completed successfully");
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database. Error: {Message}", ex.Message);
                
                // In production, you might want to throw to prevent the app from starting with a broken DB
                if (app.Environment.IsProduction())
                {
                    throw;
                }
            }
        }
    }
}
