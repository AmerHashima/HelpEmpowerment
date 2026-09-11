using System.Security.Claims;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HelpEmpowermentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseRevenueController : ControllerBase
{
    private readonly IRevenueManagementService _service;

    public CourseRevenueController(IRevenueManagementService service)
    {
        _service = service;
    }

    [HttpPost("search")]
    [Authorize]
    public async Task<ActionResult<PagedResponse<CourseRevenueDetailsDto>>> SearchCourseRevenue(
        [FromBody] DataRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var response = await _service.SearchCourseRevenueAsync(
            request, userId, User.IsInRole(SystemRoles.Admin), ct);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("{courseId}/search")]
    public async Task<ActionResult<PagedResponse<CourseRevenueShareDto>>> SearchShares(
        Guid courseId, [FromBody] DataRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var response = await _service.SearchSharesAsync(courseId, request, ct);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{courseId}/{id}")]
    public async Task<ActionResult<ApiResponse<CourseRevenueShareDto>>> GetShareById(Guid courseId, Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var response = await _service.GetShareByIdAsync(courseId, id, ct);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("{courseId}")]
    public async Task<IActionResult> CreateShare(Guid courseId, SaveCourseRevenueShareDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await _service.CreateShareAsync(courseId, dto, actorId, ct);
        return result.IsSuccess
            ? Created($"api/courses/{courseId}/revenue-shares/{result.Value!.Oid}", ApiResponse<CourseRevenueShareDto>.SuccessResponse(result.Value, "Revenue share created successfully"))
            : UnprocessableEntity(ApiResponse<CourseRevenueShareDto>.ErrorResponse(result.ErrorMessage ?? "Unable to create revenue share"));
    }

    [HttpPut("{courseId}/{id}")]
    public async Task<IActionResult> UpdateShare(Guid courseId, Guid id, SaveCourseRevenueShareDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await _service.UpdateShareAsync(courseId, id, dto, actorId, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<CourseRevenueShareDto>.SuccessResponse(result.Value!, "Revenue share updated successfully"))
            : UnprocessableEntity(ApiResponse<CourseRevenueShareDto>.ErrorResponse(result.ErrorMessage ?? "Unable to update revenue share"));
    }

    [HttpDelete("{courseId}/{id}")]
    public async Task<IActionResult> DeleteShare(Guid courseId, Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var deleted = await _service.DeleteShareAsync(courseId, id, actorId, ct);
        return deleted
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Revenue share deleted successfully"))
            : NotFound(ApiResponse<bool>.ErrorResponse("Revenue share not found"));
    }

    [HttpGet("{courseId}/summary")]
    public async Task<IActionResult> GetSummary(Guid courseId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _service.GetCourseSummaryAsync(courseId, ct);
        return result is null
            ? NotFound(ApiResponse<CourseRevenueSummaryDto>.ErrorResponse("Course not found"))
            : Ok(ApiResponse<CourseRevenueSummaryDto>.SuccessResponse(result));
    }

    private bool TryGetUserId(out Guid id) => Guid.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out id);
}
