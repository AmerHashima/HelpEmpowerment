using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseServicesController : ControllerBase
    {
        private readonly ICourseServiceDetailService _courseServiceDetailService;

        public CourseServicesController(ICourseServiceDetailService courseServiceDetailService)
        {
            _courseServiceDetailService = courseServiceDetailService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseServiceDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _courseServiceDetailService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseServiceDto>>> GetById(Guid id)
        {
            var response = await _courseServiceDetailService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CourseServiceDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _courseServiceDetailService.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseServiceDto>>> Create([FromBody] CreateCourseServiceDto dto)
        {
            var response = await _courseServiceDetailService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseServiceDto>>> Update([FromBody] UpdateCourseServiceDto dto)
        {
            var response = await _courseServiceDetailService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _courseServiceDetailService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
