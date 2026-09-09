using System.Security.Claims;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpEmpowermentApi.Controllers;

[ApiController, Route("api/user-course-assignments"), Authorize(Policy = "InternalUser")]
public sealed class UserCourseAssignmentsController(IRevenueManagementService service) : ControllerBase
{
    [HttpGet, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Get([FromQuery] Guid? userId, [FromQuery] Guid? courseId,
        [FromQuery] Guid? assignmentType, [FromQuery] bool? isActive, CancellationToken ct) =>
        Ok(await service.GetAssignmentsAsync(userId, courseId, assignmentType, isActive, ct));

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(SaveUserCourseAssignmentDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await service.CreateAssignmentAsync(dto, actorId, ct);
        return result.IsSuccess ? Created($"api/user-course-assignments/{result.Value!.Oid}", result.Value) : UnprocessableEntity(result);
    }

    [HttpPut("{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, SaveUserCourseAssignmentDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await service.UpdateAssignmentAsync(id, dto, actorId, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result);
    }

    [HttpDelete("{id:guid}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        return await service.DeleteAssignmentAsync(id, actorId, ct) ? NoContent() : NotFound();
    }

    private bool TryGetUserId(out Guid id) => Guid.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out id);
}
