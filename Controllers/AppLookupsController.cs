using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AppLookupsController : ControllerBase
    {
        private readonly IAppLookupService _lookupService;

        public AppLookupsController(IAppLookupService lookupService)
        {
            _lookupService = lookupService;
        }

        // Header endpoints
        [HttpPost("headers/search")]
        public async Task<ActionResult<PagedResponse<AppLookupHeaderDto>>> SearchHeaders([FromBody] DataRequest request)
        {
            var response = await _lookupService.GetHeadersPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("headers/{id}")]
        public async Task<ActionResult<ApiResponse<AppLookupHeaderDto>>> GetHeaderById(Guid id)
        {
            var response = await _lookupService.GetHeaderByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("headers/code/{lookupCode}")]
        public async Task<ActionResult<ApiResponse<AppLookupHeaderDto>>> GetHeaderByCode(string lookupCode)
        {
            var response = await _lookupService.GetHeaderByCodeAsync(lookupCode);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("headers")]
        public async Task<ActionResult<ApiResponse<AppLookupHeaderDto>>> CreateHeader([FromBody] CreateAppLookupHeaderDto dto)
        {
            var response = await _lookupService.CreateHeaderAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetHeaderById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut("headers")]
        public async Task<ActionResult<ApiResponse<AppLookupHeaderDto>>> UpdateHeader([FromBody] UpdateAppLookupHeaderDto dto)
        {
            var response = await _lookupService.UpdateHeaderAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("headers/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteHeader(Guid id)
        {
            var response = await _lookupService.DeleteHeaderAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        // Detail endpoints
        [HttpPost("details/search")]
        public async Task<ActionResult<PagedResponse<AppLookupDetailDto>>> SearchDetails([FromBody] DataRequest request)
        {
            var response = await _lookupService.GetDetailsPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<ApiResponse<AppLookupDetailDto>>> GetDetailById(Guid id)
        {
            var response = await _lookupService.GetDetailByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("details/header/{headerId}")]
        public async Task<ActionResult<ApiResponse<List<AppLookupDetailDto>>>> GetDetailsByHeaderId(Guid headerId)
        {
            var response = await _lookupService.GetDetailsByHeaderIdAsync(headerId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("details/header-code/{headerCode}")]
        public async Task<ActionResult<ApiResponse<List<AppLookupDetailDto>>>> GetDetailsByHeaderCode(string headerCode)
        {
            var response = await _lookupService.GetDetailsByHeaderCodeAsync(headerCode);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("details")]
        public async Task<ActionResult<ApiResponse<AppLookupDetailDto>>> CreateDetail([FromBody] CreateAppLookupDetailDto dto)
        {
            var response = await _lookupService.CreateDetailAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetDetailById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut("details")]
        public async Task<ActionResult<ApiResponse<AppLookupDetailDto>>> UpdateDetail([FromBody] UpdateAppLookupDetailDto dto)
        {
            var response = await _lookupService.UpdateDetailAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("details/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDetail(Guid id)
        {
            var response = await _lookupService.DeleteDetailAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}