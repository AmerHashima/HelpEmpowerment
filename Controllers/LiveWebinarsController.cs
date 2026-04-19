using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LiveWebinarsController : ControllerBase
    {
        private readonly ILiveWebinarService _service;

        public LiveWebinarsController(ILiveWebinarService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<LiveWebinarDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _service.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LiveWebinarDto>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<LiveWebinarDto>>> Create([FromBody] CreateLiveWebinarDto dto)
        {
            var response = await _service.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<LiveWebinarDto>>> Update([FromBody] UpdateLiveWebinarDto dto)
        {
            var response = await _service.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _service.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
