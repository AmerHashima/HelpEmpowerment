using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseInstructorsController : ControllerBase
    {
        private readonly ICourseInstructorService _instructorService;

        public CourseInstructorsController(ICourseInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseInstructorDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _instructorService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseInstructorDto>>> GetById(Guid id)
        {
            var response = await _instructorService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CourseInstructorDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _instructorService.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseInstructorDto>>> Create([FromBody] CreateCourseInstructorDto dto)
        {
            var response = await _instructorService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseInstructorDto>>> Update([FromBody] UpdateCourseInstructorDto dto)
        {
            var response = await _instructorService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _instructorService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}