using Microsoft.AspNetCore.Mvc;
using StandardArticture.Common;
using StandardArticture.DTOs;
using StandardArticture.IServices;

namespace StandardArticture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _courseService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> GetById(Guid id)
        {
            var response = await _courseService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("code/{courseCode}")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> GetByCode(string courseCode)
        {
            var response = await _courseService.GetByCodeAsync(courseCode);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Create([FromBody] CreateCourseDto dto)
        {
            var response = await _courseService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Update([FromBody] UpdateCourseDto dto)
        {
            var response = await _courseService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _courseService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}   