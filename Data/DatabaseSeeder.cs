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

            // Links are permissions, not merely menu labels. Keep every current
            // Admin sidebar destination available for assignment from Roles.
            await SeedAdminLinksAsync(dbContext, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await SeedCourseTabsAsync(dbContext, cancellationToken);
            await SeedArticlesAsync(dbContext, cancellationToken);
            logger.LogInformation("Database migrations and seed data applied successfully.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Database migration or seeding failed.");
            throw;
        }
    }

    private sealed record SeedTab(string TabKey, bool IsEnabled, int OrderNo, string Status, JsonElement Content);

    private sealed record SeedArticle(
        Guid Oid,
        string Category,
        string? CourseCode,
        string Slug,
        string TitleEn,
        string TitleAr,
        string SummaryEn,
        string SummaryAr,
        string CoverImagePath,
        DateTime PublishedAt,
        int ReadingTimeMinutes);

    private sealed record SeedLink(string NameEn, string NameAr, string Path);

    private static async Task SeedAdminLinksAsync(ApplicationDbContext db, CancellationToken ct)
    {
        var requiredLinks = new[]
        {
            new SeedLink("Dashboard", "لوحة التحكم", "/admin/dashboard"),
            new SeedLink("Reports", "التقارير", "/admin/reports"),
            new SeedLink("Certifications", "الشهادات", "/admin/certifications"),
            new SeedLink("Users", "المستخدمون", "/admin/students"),
            new SeedLink("Users Features Reservations", "حجوزات خدمات المستخدمين", "/admin/students-table"),
            new SeedLink("Messages", "الرسائل", "/admin/contact-us"),
            new SeedLink("Live Course", "دورة مباشرة", "/admin/live-course"),
            new SeedLink("Webinar", "ندوة عبر الإنترنت", "/admin/webinar"),
            new SeedLink("Announcements", "الإعلانات", "/admin/announcements"),
            new SeedLink("Moderators", "المشرفون", "/admin/moderators"),
            new SeedLink("Roles", "الأدوار", "/admin/roles"),
            new SeedLink("Articles", "المقالات", "/admin/articles")
        };

        var existingPaths = (await db.Links
                .IgnoreQueryFilters()
                .Select(link => new { link.Path, link.IsDeleted })
                .ToListAsync(ct))
            .Where(link => !link.IsDeleted)
            .Select(link => NormalizeAdminPath(link.Path))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var link in requiredLinks.Where(link => !existingPaths.Contains(NormalizeAdminPath(link.Path))))
        {
            db.Links.Add(new Link
            {
                NameEn = link.NameEn,
                NameAr = link.NameAr,
                Path = link.Path,
                Active = true,
                IsActive = true
            });
        }
    }

    private static string NormalizeAdminPath(string? path)
    {
        var normalized = $"/{(path ?? string.Empty).Trim().Trim('/')}".ToLowerInvariant();
        return normalized.StartsWith("/admin/", StringComparison.Ordinal)
            ? normalized
            : $"/admin{normalized}";
    }

    private static async Task SeedArticlesAsync(ApplicationDbContext db, CancellationToken ct)
    {
        var assembly = typeof(DatabaseSeeder).Assembly;
        var resource = assembly.GetManifestResourceNames()
            .Single(name => name.EndsWith(".articles.json", StringComparison.Ordinal));
        await using var stream = assembly.GetManifestResourceStream(resource)!;
        var contentBySlug = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(
            stream, new JsonSerializerOptions(JsonSerializerDefaults.Web), ct) ?? [];

        var articles = new[]
        {
            new SeedArticle(
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                "capm", "CAPM", "capm-study-plan",
                "CAPM Study Plan", "خطة دراسة CAPM",
                "Passing the CAPM exam requires a structured methodology, much like managing a real project. This guide breaks your preparation into five clear phases, helping you build strong understanding, practice effectively, and confidently prepare for the exam.",
                "يتطلب النجاح في اختبار CAPM اتباع منهجية منظمة مثل إدارة مشروع حقيقي. يوضح هذا الدليل خمس مراحل أساسية تساعدك على الفهم والتطبيق والاستعداد بثقة للاختبار.",
                "assets/images/homeArticles/article1.jpeg",
                new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc), 8),
            new SeedArticle(
                Guid.Parse("10000000-0000-0000-0000-000000000002"),
                "pmp", "PMP", "pmp-study-plan",
                "PMP Study Plan", "خطة دراسة PMP",
                "Passing the PMP exam requires a structured methodology, much like managing a real project. Based on experience with many candidates, this guide divides your preparation into five clear phases.",
                "اجتياز امتحان PMP يتطلب منهجية منظمة، تمامًا مثل إدارة مشروع حقيقي. بناءً على خبرة العديد من المرشحين، يقسم هذا الدليل التحضير إلى خمس مراحل واضحة.",
                "assets/images/homeArticles/article2.jpeg",
                new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc), 6),
            new SeedArticle(
                Guid.Parse("10000000-0000-0000-0000-000000000003"),
                "general", null, "digital-transformation-failure",
                "Three Reasons Why Most Digital Transformation Plans Fail", "ثلاثة أسباب تجعل أغلب خطط التحول الرقمي تفشل",
                "70% of digital transformation initiatives fail to achieve their goals despite massive investments—and the reason is rarely technical. This article explores digital patching, loss-driven decisions, and resistance to transparency.",
                "70% من مبادرات التحول الرقمي تفشل رغم الاستثمارات الضخمة، والسبب غالباً ليس تقنياً. يستعرض هذا المقال الترقيع الرقمي والقرارات المبنية على الخوف من الخسارة ومقاومة الشفافية.",
                "assets/images/homeArticles/article3.png",
                new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc), 6),
            new SeedArticle(
                Guid.Parse("10000000-0000-0000-0000-000000000004"),
                "capm", "CAPM", "capm-application-process",
                "CAPM Application Process", "التقديم على شهادة CAPM",
                "Applying for the CAPM certification is simple and requires only basic education and 23 hours of training. This guide walks through creating a PMI account, completing the application, and scheduling the exam.",
                "التقديم على CAPM بسيط ويتطلب مؤهلًا دراسيًا و23 ساعة تدريب فقط. يوضح هذا الدليل خطوات إنشاء حساب PMI وإتمام الطلب وحجز الامتحان.",
                "assets/images/homeArticles/article1.jpeg",
                new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc), 8)
        };

        var slugs = articles.Select(article => article.Slug).ToArray();
        var existing = await db.Articles
            .IgnoreQueryFilters()
            .AsTracking()
            .Where(article => slugs.Contains(article.Slug))
            .ToListAsync(ct);

        foreach (var article in articles)
        {
            if (!contentBySlug.TryGetValue(article.Slug, out var blocks))
                throw new InvalidOperationException($"Legacy content is missing for article '{article.Slug}'.");

            var contentJson = blocks.GetRawText();
            var current = existing.SingleOrDefault(item => item.Slug == article.Slug);
            if (current is not null)
            {
                // Upgrade only the deterministic seed rows while they are still untouched.
                // Dashboard edits set UpdatedAt/UpdatedBy and must never be overwritten.
                if (current.Oid == article.Oid && current.UpdatedAt is null && current.UpdatedBy is null)
                    current.ContentJson = contentJson;
                continue;
            }

            db.Articles.Add(new Article
            {
                Oid = article.Oid,
                Category = article.Category,
                CourseCode = article.CourseCode,
                Slug = article.Slug,
                TitleEn = article.TitleEn,
                TitleAr = article.TitleAr,
                SummaryEn = article.SummaryEn,
                SummaryAr = article.SummaryAr,
                CoverImagePath = article.CoverImagePath,
                CoverImageAltEn = article.TitleEn,
                CoverImageAltAr = article.TitleAr,
                PublishedAt = article.PublishedAt,
                ReadingTimeMinutes = article.ReadingTimeMinutes,
                Status = "Published",
                IsFeatured = true,
                ContentJson = contentJson
            });
        }

        await db.SaveChangesAsync(ct);
    }

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
