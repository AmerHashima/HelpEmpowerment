using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Models;
using System.Reflection;
using System.Text.Json;

namespace HelpEmpowermentApi.Data;

/// <summary>
/// Initializes the application database by applying pending Entity Framework
/// migrations. Migration execution also installs the deterministic lookup data
/// configured with <c>HasData</c> in <see cref="ApplicationDbContext"/>.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Applies all pending migrations and their seed-data operations.
    /// The method may be called repeatedly; EF only applies migrations that are
    /// not recorded in <c>__EFMigrationsHistory</c>.
    /// </summary>
    /// <param name="services">The application's root service provider.</param>
    /// <param name="cancellationToken">Token used to cancel database work.</param>
    public static async Task SeedAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Applying migrations also applies every HasData seed declared in
            // ApplicationDbContext, and is safe to run when the database is current.
            await dbContext.Database.MigrateAsync(cancellationToken);

            // Keep application roles available even when a database was created
            // before role seed data was introduced.
            var requiredRoles = new[]
            {
                new { Id = Common.SystemRoles.AdminId, Name = Common.SystemRoles.Admin, Description = "Application administrator" },
                new { Id = Common.SystemRoles.TrainerId, Name = Common.SystemRoles.Trainer, Description = "Course trainer" }
            };

            foreach (var requiredRole in requiredRoles)
            {
                if (!await dbContext.Roles.AnyAsync(
                        role => role.Name.ToLower() == requiredRole.Name.ToLower() && !role.IsDeleted,
                        cancellationToken))
                {
                    dbContext.Roles.Add(new Role
                    {
                        Oid = requiredRole.Id,
                        Name = requiredRole.Name,
                        Description = requiredRole.Description,
                        IsActive = true
                    });
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await SeedCourseTabsAsync(dbContext, cancellationToken);
            logger.LogInformation("Database migrations and seed data applied successfully.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Database migration or seeding failed.");
            throw;
        }
    }

    private sealed record SeedTab(string TabKey, bool IsEnabled, int OrderNo, string Status, JsonElement Content);

    private static async Task SeedCourseTabsAsync(ApplicationDbContext db, CancellationToken ct)
    {
        var assembly = typeof(DatabaseSeeder).Assembly;
        foreach (var code in new[] { "PMP", "CAPM" })
        {
            var resource = assembly.GetManifestResourceNames()
                .Single(name => name.EndsWith($".{code.ToLowerInvariant()}-tabs.json", StringComparison.Ordinal));
            await using var stream = assembly.GetManifestResourceStream(resource)!;
            var tabs = await JsonSerializer.DeserializeAsync<SeedTab[]>(stream,
                new JsonSerializerOptions(JsonSerializerDefaults.Web), ct) ?? [];
            var existing = await db.CourseTabContents.AsTracking()
                .Where(x => !x.IsDeleted && x.CourseCode == code).ToListAsync(ct);
            foreach (var tab in tabs)
            {
                var row = existing.FirstOrDefault(x => x.TabKey == tab.TabKey);
                if (row is not null)
                {
                    // Upgrade only untouched placeholders created by the earlier GET-time defaults.
                    if (row.UpdatedAt is null && row.CreatedBy is null &&
                        (row.ContentJson.Contains("Everything you need for your") ||
                         row.ContentJson.Contains("\"titlePart1\":\"\"")))
                        row.ContentJson = tab.Content.GetRawText();
                    continue; // Never overwrite dashboard edits.
                }
                db.CourseTabContents.Add(new CourseTabContent
                {
                    CourseCode = code, TabKey = tab.TabKey, IsEnabled = tab.IsEnabled,
                    OrderNo = tab.OrderNo, Status = tab.Status,
                    ContentJson = tab.Content.GetRawText()
                });
            }
        }
        await db.SaveChangesAsync(ct);
    }
}
