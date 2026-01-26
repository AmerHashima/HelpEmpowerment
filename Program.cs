using Microsoft.EntityFrameworkCore;
using StandardArticture.Data;
using StandardArticture.Extensions;
using Polly;


namespace StandardArticture
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add application services
            builder.Services.AddApplicationServices(builder.Configuration);

            // Add response compression for better performance
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // Add HttpClient with Polly resiliency policies
            builder.Services.AddHttpClient("MyHttpClient")
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(10)) // 10 seconds timeout
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))) // Exponential backoff
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder
                    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))); // Break circuit after 5 failed attempts for 30 seconds

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
