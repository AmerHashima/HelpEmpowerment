using System.Security.Claims;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpEmpowermentApi.Controllers;

[ApiController, Route("api/me"), Authorize(Policy = "InternalUser")]
public sealed class MeController(IRevenueManagementService service) : ControllerBase
{
    [HttpGet("courses")]
    public async Task<IActionResult> Courses(CancellationToken ct)
    {
        if (!TryGetInternalUserId(out var userId)) return Forbid();
        return Ok(await service.GetUserCoursesAsync(userId, ct));
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        if (!TryGetInternalUserId(out var userId)) return Forbid();
        return Ok(await service.GetDashboardAsync(userId, User.IsInRole("Admin"), ct));
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> Revenue([FromQuery] Guid? courseId, [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo, [FromQuery] RevenueDistributionStatus? status, CancellationToken ct)
    {
        if (!TryGetInternalUserId(out var userId)) return Forbid();
        return Ok(await service.GetMyRevenueAsync(userId, courseId, dateFrom, dateTo, status, ct));
    }

    private bool TryGetInternalUserId(out Guid id)
    {
        id = default;
        return string.Equals(User.FindFirstValue("UserType"), "User", StringComparison.Ordinal)
            && Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out id);
    }
}
