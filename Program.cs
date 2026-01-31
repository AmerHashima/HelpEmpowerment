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
            // Logging
            // --------------------------
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // --------------------------
            // Add services
            // --------------------------
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Application & Infrastructure services
            builder.Services.AddApplicationServices(builder.Configuration);

            // --------------------------
            // CORS Policy for Angular Dev
            // --------------------------
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:4200",
                        "http://localhost:4201",
                        "http://127.0.0.1:4200"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                    // .AllowCredentials(); // فقط إذا تستخدم Authentication مع Angular
                });
            });

            // --------------------------
            // Health Checks
            // --------------------------
            builder.Services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
                    name: "sql-server",
                    timeout: TimeSpan.FromSeconds(30),
                    tags: new[] { "ready", "db" });

            // --------------------------
            // Response Compression
            // --------------------------
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // --------------------------
            // HttpClient with Polly
            // --------------------------
            builder.Services.AddHttpClient("MyHttpClient")
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(10))
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder
                    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            var app = builder.Build();

            // --------------------------
            // Apply database migrations
            // --------------------------
            await ApplyDatabaseMigrationsAsync(app);

            // --------------------------
            // Middleware pipeline
            // --------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StandardArticture API V1");
                });
            }

            // Health check endpoints
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.UseHttpsRedirection();

            app.UseRouting();                    // Must be BEFORE UseCors
            app.UseCors("AllowAngularDev");       // Must be AFTER UseRouting, BEFORE Authorization

            app.UseAuthentication();             // If you have auth
            app.UseAuthorization();

            // Handle OPTIONS preflight (optional, ensures no CORS issues)
            app.Use(async (context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                    await context.Response.CompleteAsync();
                }
                else
                {
                    await next();
                }
            });

            app.MapControllers();

            app.Run();
        }

        // --------------------------
        // Database Migration with retry
        // --------------------------
        private static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                logger.LogInformation("Starting database migration...");

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
                    await context.Database.CanConnectAsync();
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migration completed successfully");
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database. Error: {Message}", ex.Message);
                if (app.Environment.IsProduction())
                {
                    throw;
                }
            }
        }
    }
}
