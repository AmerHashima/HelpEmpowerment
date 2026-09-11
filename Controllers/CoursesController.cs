using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HelpEmpowermentApi.Controllers
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
        [Authorize]
        public async Task<ActionResult<PagedResponse<CourseDto>>> Search([FromBody] DataRequest request)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var response = await _courseService.GetPagedAsync(request, isAdmin ? null : userId);
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
