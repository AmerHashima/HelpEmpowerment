using System.Security.Claims;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpEmpowermentApi.Controllers;

[ApiController, Route("api/revenue-settlements"), Authorize(Policy = "InternalUser", Roles = "Admin")]
public sealed class RevenueSettlementsController(IRevenueManagementService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? beneficiaryUserId,
        [FromQuery] RevenueSettlementStatus? status, CancellationToken ct) =>
        Ok(await service.GetSettlementsAsync(beneficiaryUserId, status, ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateRevenueSettlementDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await service.CreateSettlementAsync(dto, actorId, ct);
        return result.IsSuccess ? Created($"api/revenue-settlements/{result.Value!.Oid}", result.Value) : UnprocessableEntity(result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateRevenueSettlementStatusDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var actorId)) return Unauthorized();
        var result = await service.UpdateSettlementStatusAsync(id, dto, actorId, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result);
    }

    private bool TryGetUserId(out Guid id) => Guid.TryParse(
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out id);
}
