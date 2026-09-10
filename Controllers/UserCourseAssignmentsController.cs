using System.Security.Claims;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace HelpEmpowermentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserCourseAssignmentsController : ControllerBase
{
    private readonly IRevenueManagementService _service;

    public UserCourseAssignmentsController(IRevenueManagementService service)
    {
        _service = service;
    }

    [HttpPost("search")]
    public async Task<ActionResult<PagedResponse<UserCourseAssignmentDto>>> Search(
        [FromBody] DataRequest request, CancellationToken ct)
    {
        var response = await _service.SearchAssignmentsAsync(request, ct);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserCourseAssignmentDto>>> GetById(Guid id, CancellationToken ct)
    {
        var response = await _service.GetAssignmentByIdAsync(id, ct);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveUserCourseAssignmentDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await _service.CreateAssignmentAsync(dto, actorId, ct);
        return result.IsSuccess
            ? Created($"api/user-course-assignments/{result.Value!.Oid}", ApiResponse<UserCourseAssignmentDto>.SuccessResponse(result.Value, "Assignment created successfully"))
            : UnprocessableEntity(ApiResponse<UserCourseAssignmentDto>.ErrorResponse(result.ErrorMessage ?? "Unable to create assignment"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, SaveUserCourseAssignmentDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await _service.UpdateAssignmentAsync(id, dto, actorId, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<UserCourseAssignmentDto>.SuccessResponse(result.Value!, "Assignment updated successfully"))
            : UnprocessableEntity(ApiResponse<UserCourseAssignmentDto>.ErrorResponse(result.ErrorMessage ?? "Unable to update assignment"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var deleted = await _service.DeleteAssignmentAsync(id, actorId, ct);
        return deleted
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Assignment deleted successfully"))
            : NotFound(ApiResponse<bool>.ErrorResponse("Assignment not found"));
    }

    private bool TryGetUserId(out Guid id) => Guid.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out id);
}
