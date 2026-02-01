namespace HelpEmpowermentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            
            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS services
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
                app.UseSwaggerUI();
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
