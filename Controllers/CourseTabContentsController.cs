using System.Text.Json;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpEmpowermentApi.Controllers;

[ApiController]
[Route("api/course-tab-contents")]
public class CourseTabContentsController(ApplicationDbContext db) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{courseCode}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<IReadOnlyList<CourseTabContentDto>>> GetCourse(
        string courseCode, [FromQuery] string status = "Published", CancellationToken ct = default)
    {
        if (!status.Equals("Published", StringComparison.OrdinalIgnoreCase) &&
            !(User.Identity?.IsAuthenticated == true && User.HasClaim("UserType", "User")))
            return Forbid();
        var query = db.CourseTabContents.AsNoTracking()
            .Where(x => !x.IsDeleted && x.CourseCode == courseCode.Trim().ToUpper());
        if (!status.Equals("all", StringComparison.OrdinalIgnoreCase))
            query = query.Where(x => x.Status == status);
        var rows = await query.OrderBy(x => x.OrderNo).ToListAsync(ct);
        Response.Headers.CacheControl = "no-store";
        return Ok(rows.Select(Map));
    }

    [AllowAnonymous]
    [HttpGet("{courseCode}/{tabKey}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<ActionResult<CourseTabContentDto>> GetTab(string courseCode, string tabKey, CancellationToken ct)
    {
        var row = await db.CourseTabContents.AsNoTracking().FirstOrDefaultAsync(x =>
            !x.IsDeleted && x.CourseCode == courseCode.Trim().ToUpper() && x.TabKey == tabKey && x.Status == "Published", ct);
        return row is null ? NotFound() : Ok(Map(row));
    }

    [Authorize(Policy = "InternalUser")]
    [HttpPut]
    public async Task<ActionResult<CourseTabContentDto>> Upsert(UpdateCourseTabContentDto dto, CancellationToken ct)
    {
        if (!CourseTabDefaults.TabKeys.Contains(dto.TabKey)) return BadRequest("Unknown tab key.");
        if (dto.Content.ValueKind != JsonValueKind.Object ||
            !dto.Content.TryGetProperty("banner", out var banner) || banner.ValueKind != JsonValueKind.Object ||
            !banner.TryGetProperty("en", out var en) || en.ValueKind != JsonValueKind.Object ||
            !banner.TryGetProperty("ar", out var ar) || ar.ValueKind != JsonValueKind.Object ||
            !dto.Content.TryGetProperty("sections", out var sections) || sections.ValueKind != JsonValueKind.Array)
            return BadRequest("Content must include banner.en, banner.ar and a sections array.");
        if (sections.GetArrayLength() > 100)
            return BadRequest("Too many sections.");
        foreach (var section in sections.EnumerateArray())
        {
            if (section.ValueKind != JsonValueKind.Object ||
                !section.TryGetProperty("type", out var type) || type.ValueKind != JsonValueKind.String)
                return BadRequest("Every section needs a type.");
            if (type.GetString() != "custom") continue;
            if (!section.TryGetProperty("id", out var id) || id.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(id.GetString()) ||
                !section.TryGetProperty("header", out var header) || header.ValueKind != JsonValueKind.Object ||
                !section.TryGetProperty("description", out var description) || description.ValueKind != JsonValueKind.Object ||
                !HasLocalizedText(header, 150) || !HasLocalizedText(description, 5000))
                return BadRequest("Custom sections require an ID and English/Arabic header and description.");
        }
        var code = dto.CourseCode.Trim().ToUpperInvariant();
        if (!await db.Courses.AnyAsync(x => !x.IsDeleted && x.CourseCode == code, ct))
            return NotFound("Course not found.");
        var row = await db.CourseTabContents.AsTracking().FirstOrDefaultAsync(x =>
            !x.IsDeleted && x.CourseCode == code && x.TabKey == dto.TabKey, ct);
        if (row is null)
        {
            row = new CourseTabContent { CourseCode = code, TabKey = dto.TabKey, CreatedBy = dto.UpdatedBy };
            db.CourseTabContents.Add(row);
        }
        row.ContentJson = dto.Content.GetRawText();
        row.IsEnabled = dto.IsEnabled;
        row.OrderNo = dto.OrderNo;
        row.Status = dto.Status;
        row.UpdatedBy = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var userId)
            ? userId : null;
        row.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return Ok(Map(row));
    }

    private static CourseTabContentDto Map(CourseTabContent row) => new()
    {
        Oid = row.Oid, CourseCode = row.CourseCode, TabKey = row.TabKey,
        IsEnabled = row.IsEnabled, OrderNo = row.OrderNo, Status = row.Status,
        Content = JsonSerializer.Deserialize<JsonElement>(row.ContentJson), UpdatedAt = row.UpdatedAt
    };

    private static bool HasLocalizedText(JsonElement value, int maxLength) =>
        value.TryGetProperty("en", out var en) && en.ValueKind == JsonValueKind.String &&
        !string.IsNullOrWhiteSpace(en.GetString()) && en.GetString()!.Length <= maxLength &&
        value.TryGetProperty("ar", out var ar) && ar.ValueKind == JsonValueKind.String &&
        !string.IsNullOrWhiteSpace(ar.GetString()) && ar.GetString()!.Length <= maxLength;
}
