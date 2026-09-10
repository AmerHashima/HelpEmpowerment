using System.Security.Claims;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpEmpowermentApi.Controllers;

[ApiController, Route("api/me"), Authorize(Policy = "InternalUser")]
public sealed class MeController(IRevenueManagementService service) : ControllerBase
{
    [HttpPost("courses/search")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AssignedCourseDto>>>> Courses(CancellationToken ct)
    {
        if (!TryGetInternalUserId(out var userId)) return Forbid();
        var data = await service.GetUserCoursesAsync(userId, ct);
        return Ok(ApiResponse<IReadOnlyList<AssignedCourseDto>>.SuccessResponse(data));
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        if (!TryGetInternalUserId(out var userId)) return Forbid();
        var data = await service.GetDashboardAsync(userId, User.IsInRole("Admin"), ct);
        return Ok(ApiResponse<AssignedDashboardDto>.SuccessResponse(data));
    }

    [HttpPost("revenue/search")]
    public async Task<ActionResult<ApiResponse<MyRevenueDto>>> Revenue(
        [FromBody] MyRevenueSearchDto request, CancellationToken ct)
    {
        if (!TryGetInternalUserId(out var userId)) return Forbid();
        var data = await service.GetMyRevenueAsync(userId, request.CourseId, request.DateFrom,
            request.DateTo, request.Status, ct);
        return Ok(ApiResponse<MyRevenueDto>.SuccessResponse(data));
    }

    private bool TryGetInternalUserId(out Guid id)
    {
        id = default;
        return string.Equals(User.FindFirstValue("UserType"), "User", StringComparison.Ordinal)
            && Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out id);
    }
}
