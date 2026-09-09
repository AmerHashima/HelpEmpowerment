using System.Security.Claims;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpEmpowermentApi.Controllers;

[ApiController, Route("api/courses/{courseId:guid}"), Authorize(Policy = "InternalUser")]
public sealed class CourseRevenueController(IRevenueManagementService service) : ControllerBase
{
    [HttpGet("revenue-shares")]
    public async Task<IActionResult> GetShares(Guid courseId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        if (!await service.CanAccessCourseAsync(userId, courseId, User.IsInRole("Admin"), ct)) return Forbid();
        return Ok(await service.GetSharesAsync(courseId, ct));
    }

    [HttpPost("revenue-shares"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateShare(Guid courseId, SaveCourseRevenueShareDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await service.CreateShareAsync(courseId, dto, actorId, ct);
        return result.IsSuccess ? Created($"api/courses/{courseId}/revenue-shares/{result.Value!.Oid}", result.Value) : UnprocessableEntity(result);
    }

    [HttpPut("revenue-shares/{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateShare(Guid courseId, Guid id, SaveCourseRevenueShareDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await service.UpdateShareAsync(courseId, id, dto, actorId, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result);
    }

    [HttpDelete("revenue-shares/{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteShare(Guid courseId, Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        return await service.DeleteShareAsync(courseId, id, actorId, ct) ? NoContent() : NotFound();
    }

    [HttpGet("revenue-summary")]
    public async Task<IActionResult> GetSummary(Guid courseId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        if (!await service.CanAccessCourseAsync(userId, courseId, User.IsInRole("Admin"), ct)) return Forbid();
        var result = await service.GetCourseSummaryAsync(courseId, ct);
        return result is null ? NotFound() : Ok(result);
    }

    private bool TryGetUserId(out Guid id) => Guid.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out id);
}
