using System.Security.Claims;
using System.Text.Json;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpEmpowermentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController(ApplicationDbContext db, IConfiguration configuration) : ControllerBase
{
    private static readonly HashSet<string> AllowedBlockTypes = new(StringComparer.OrdinalIgnoreCase)
        { "main-header", "sub-header", "details", "bullet-list", "numbered-list", "image" };
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private string ImagesPath => configuration["FileStorage:ArticleImagesPath"] ?? "/var/www/images/articles";

    [AllowAnonymous]
    [HttpPost("search")]
    public async Task<ActionResult<PagedResponse<ArticleDto>>> Search([FromBody] DataRequest request, CancellationToken ct)
    {
        var isInternal = User.Identity?.IsAuthenticated == true && User.HasClaim("UserType", "User");
        IQueryable<Article> query = db.Articles.AsNoTracking().Where(x => !x.IsDeleted);
        if (!isInternal) query = query.Where(x => x.Status == "Published" && (!x.PublishedAt.HasValue || x.PublishedAt <= DateTime.UtcNow));

        foreach (var filter in request.Filters)
        {
            var name = filter.PropertyName.Trim().ToLowerInvariant();
            var value = filter.Value?.Trim();
            if (string.IsNullOrWhiteSpace(value)) continue;
            if (name is "category" or "coursecode")
                query = name == "category" ? query.Where(x => x.Category == value) : query.Where(x => x.CourseCode == value);
            else if (name is "title" or "titleen" or "search")
                query = query.Where(x => x.TitleEn.Contains(value) || x.TitleAr.Contains(value) || (x.SummaryEn ?? "").Contains(value) || (x.SummaryAr ?? "").Contains(value));
            else if (name == "status" && isInternal) query = query.Where(x => x.Status == value);
            else if (name == "featured" && bool.TryParse(value, out var featured)) query = query.Where(x => x.IsFeatured == featured);
        }

        query = request.Sort.FirstOrDefault()?.SortBy?.ToLowerInvariant() switch
        {
            "title" or "titleen" => request.Sort[0].SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) ? query.OrderBy(x => x.TitleEn) : query.OrderByDescending(x => x.TitleEn),
            "publishedat" => request.Sort[0].SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) ? query.OrderBy(x => x.PublishedAt) : query.OrderByDescending(x => x.PublishedAt),
            _ => query.OrderByDescending(x => x.IsFeatured).ThenByDescending(x => x.PublishedAt).ThenByDescending(x => x.CreatedAt)
        };
        var total = await query.CountAsync(ct);
        var pageSize = request.Pagination.GetAll ? Math.Max(total, 1) : Math.Clamp(request.Pagination.PageSize, 1, 100);
        var page = Math.Max(request.Pagination.PageNumber, 1);
        var rows = await query.Skip(request.Pagination.GetAll ? 0 : (page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(new PagedResponse<ArticleDto> { Success = true, Data = rows.Select(Map).ToList(), TotalCount = total, PageNumber = page, PageSize = pageSize });
    }

    [AllowAnonymous]
    [HttpGet("by-slug/{category}/{slug}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> GetBySlug(string category, string slug, CancellationToken ct)
    {
        var article = await db.Articles.AsNoTracking().FirstOrDefaultAsync(x => !x.IsDeleted && x.Category == category && x.Slug == slug && x.Status == "Published" && (!x.PublishedAt.HasValue || x.PublishedAt <= DateTime.UtcNow), ct);
        return article is null ? NotFound(ApiResponse<ArticleDto>.ErrorResponse("Article not found.")) : Ok(ApiResponse<ArticleDto>.SuccessResponse(Map(article)));
    }

    [Authorize(Policy = "InternalUser")]
    [HttpGet("{id:guid}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> Get(Guid id, CancellationToken ct)
    {
        var article = await db.Articles.AsNoTracking().FirstOrDefaultAsync(x => x.Oid == id && !x.IsDeleted, ct);
        return article is null ? NotFound(ApiResponse<ArticleDto>.ErrorResponse("Article not found.")) : Ok(ApiResponse<ArticleDto>.SuccessResponse(Map(article)));
    }

    [Authorize(Policy = "InternalUser")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> Create([FromBody] SaveArticleDto dto, CancellationToken ct)
    {
        var validation = Validate(dto); if (validation is not null) return BadRequest(ApiResponse<ArticleDto>.ErrorResponse(validation));
        var category = Normalize(dto.Category); var slug = Normalize(dto.Slug);
        if (await db.Articles.AnyAsync(x => !x.IsDeleted && x.Category == category && x.Slug == slug, ct)) return Conflict(ApiResponse<ArticleDto>.ErrorResponse("An article with this category and slug already exists."));
        var article = new Article { CreatedBy = CurrentUserId() }; Apply(article, dto); db.Articles.Add(article);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = article.Oid }, ApiResponse<ArticleDto>.SuccessResponse(Map(article), "Article created."));
    }

    [Authorize(Policy = "InternalUser")]
    [HttpPut]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> Update([FromBody] UpdateArticleDto dto, CancellationToken ct)
    {
        var validation = Validate(dto); if (validation is not null) return BadRequest(ApiResponse<ArticleDto>.ErrorResponse(validation));
        // Legacy deletes were soft deletes. An administrator editing one of those
        // records should restore it instead of receiving a misleading 404.
        // The application uses NoTracking globally, so this row must be tracked
        // or SaveChanges would only return the in-memory values without persisting them.
        var article = await db.Articles.AsTracking().FirstOrDefaultAsync(x => x.Oid == dto.Oid, ct);
        if (article is null) return NotFound(ApiResponse<ArticleDto>.ErrorResponse("Article not found."));
        var category = Normalize(dto.Category); var slug = Normalize(dto.Slug);
        if (await db.Articles.AnyAsync(x => !x.IsDeleted && x.Oid != dto.Oid && x.Category == category && x.Slug == slug, ct)) return Conflict(ApiResponse<ArticleDto>.ErrorResponse("An article with this category and slug already exists."));
        var wasSoftDeleted = article.IsDeleted;
        Apply(article, dto);
        article.IsDeleted = false;
        article.DeletedAt = null;
        article.UpdatedAt = DateTime.UtcNow;
        article.UpdatedBy = CurrentUserId();
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<ArticleDto>.SuccessResponse(Map(article), wasSoftDeleted ? "Article restored and updated." : "Article updated."));
    }

    [Authorize(Policy = "InternalUser")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken ct)
    {
        // Articles are managed content: deleting from the dashboard must remove the
        // record entirely, including legacy records that were soft-deleted before.
        var article = await db.Articles.FirstOrDefaultAsync(x => x.Oid == id, ct);
        if (article is null) return NotFound(ApiResponse<bool>.ErrorResponse("Article not found."));
        db.Articles.Remove(article);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Article permanently deleted."));
    }

    [Authorize(Policy = "InternalUser")]
    [HttpPost("images")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<object>> UploadImage([FromForm] ArticleImageUploadRequest request, CancellationToken ct)
    {
        var image = request.Image;
        if (image is null || image.Length == 0) return BadRequest("A non-empty image is required.");
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension)) return BadRequest("Only JPG, PNG, GIF, and WEBP images are allowed.");
        Directory.CreateDirectory(ImagesPath);
        var fileName = $"article_{Guid.NewGuid():N}{extension}";
        await using var stream = new FileStream(Path.Combine(ImagesPath, fileName), FileMode.CreateNew, FileAccess.Write);
        await image.CopyToAsync(stream, ct);
        return Ok(new { fileName });
    }

    [AllowAnonymous]
    [HttpGet("images/{fileName}")]
    public IActionResult GetImage(string fileName)
    {
        if (!string.Equals(fileName, Path.GetFileName(fileName), StringComparison.Ordinal)) return BadRequest();
        var path = Path.Combine(ImagesPath, fileName); if (!System.IO.File.Exists(path)) return NotFound();
        var mime = Path.GetExtension(fileName).ToLowerInvariant() switch { ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".gif" => "image/gif", ".webp" => "image/webp", _ => "application/octet-stream" };
        return PhysicalFile(path, mime);
    }

    private static ArticleDto Map(Article item) => new()
    {
        Oid = item.Oid, Category = item.Category, CourseCode = item.CourseCode, Slug = item.Slug, TitleEn = item.TitleEn, TitleAr = item.TitleAr,
        SummaryEn = item.SummaryEn, SummaryAr = item.SummaryAr, CoverImagePath = item.CoverImagePath, CoverImageAltEn = item.CoverImageAltEn, CoverImageAltAr = item.CoverImageAltAr,
        PublishedAt = item.PublishedAt, ReadingTimeMinutes = item.ReadingTimeMinutes, Status = item.Status, IsFeatured = item.IsFeatured, CreatedAt = item.CreatedAt, UpdatedAt = item.UpdatedAt,
        Blocks = JsonSerializer.Deserialize<List<ArticleBlockDto>>(item.ContentJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? []
    };
    private static void Apply(Article item, SaveArticleDto dto)
    {
        item.Category = Normalize(dto.Category); item.CourseCode = string.IsNullOrWhiteSpace(dto.CourseCode) ? null : dto.CourseCode.Trim().ToUpperInvariant(); item.Slug = Normalize(dto.Slug);
        item.TitleEn = dto.TitleEn.Trim(); item.TitleAr = dto.TitleAr.Trim(); item.SummaryEn = dto.SummaryEn?.Trim(); item.SummaryAr = dto.SummaryAr?.Trim(); item.CoverImagePath = dto.CoverImagePath?.Trim(); item.CoverImageAltEn = dto.CoverImageAltEn?.Trim(); item.CoverImageAltAr = dto.CoverImageAltAr?.Trim(); item.PublishedAt = dto.PublishedAt; item.ReadingTimeMinutes = dto.ReadingTimeMinutes; item.Status = dto.Status; item.IsFeatured = dto.IsFeatured;
        item.ContentJson = JsonSerializer.Serialize(dto.Blocks.OrderBy(x => x.OrderNo));
    }
    private static string? Validate(SaveArticleDto dto)
    {
        if (dto.Blocks.Count > 250) return "An article may contain at most 250 content blocks.";
        if (dto.Blocks.Any(x => !AllowedBlockTypes.Contains(x.Type) || (x.Type != "image" && string.IsNullOrWhiteSpace(x.TextEn) && string.IsNullOrWhiteSpace(x.TextAr) && !x.ItemsEn.Any() && !x.ItemsAr.Any()) || (x.Type == "image" && string.IsNullOrWhiteSpace(x.ImagePath)))) return "Each block must have a valid type and content; image blocks require an image path.";
        return null;
    }
    private Guid? CurrentUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id) ? id : null;
    private static string Normalize(string value) => value.Trim().ToLowerInvariant().Replace(' ', '-');
}

public sealed class ArticleImageUploadRequest
{
    public IFormFile? Image { get; set; }
}
