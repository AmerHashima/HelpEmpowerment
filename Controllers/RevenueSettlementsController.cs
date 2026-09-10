using System.Security.Claims;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HelpEmpowermentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RevenueSettlementsController : ControllerBase
{
    private readonly IRevenueManagementService _service;

    public RevenueSettlementsController(IRevenueManagementService service)
    {
        _service = service;
    }
    [HttpPost("search")]
    public async Task<ActionResult<PagedResponse<RevenueSettlementDto>>> Search(
        [FromBody] DataRequest request, CancellationToken ct)
    {
        var response = await _service.SearchSettlementsAsync(request, ct);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RevenueSettlementDto>>> GetById(Guid id, CancellationToken ct)
    {
        var response = await _service.GetSettlementByIdAsync(id, ct);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRevenueSettlementDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await _service.CreateSettlementAsync(dto, actorId, ct);
        return result.IsSuccess
            ? Created($"api/revenue-settlements/{result.Value!.Oid}", ApiResponse<RevenueSettlementDto>.SuccessResponse(result.Value, "Settlement created successfully"))
            : UnprocessableEntity(ApiResponse<RevenueSettlementDto>.ErrorResponse(result.ErrorMessage ?? "Unable to create settlement"));
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateRevenueSettlementStatusDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await _service.UpdateSettlementStatusAsync(id, dto, actorId, ct);
        return result.IsSuccess
            ? Ok(ApiResponse<RevenueSettlementDto>.SuccessResponse(result.Value!, "Settlement status updated successfully"))
            : UnprocessableEntity(ApiResponse<RevenueSettlementDto>.ErrorResponse(result.ErrorMessage ?? "Unable to update settlement"));
    }

    private bool TryGetUserId(out Guid id) => Guid.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out id);
}
