using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LiveCoursesController : ControllerBase
    {
        private readonly ILiveCourseService _service;

        public LiveCoursesController(ILiveCourseService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<LiveCourseDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _service.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LiveCourseDto>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<LiveCourseDto>>> Create([FromBody] CreateLiveCourseDto dto)
        {
            var response = await _service.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<LiveCourseDto>>> Update([FromBody] UpdateLiveCourseDto dto)
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
