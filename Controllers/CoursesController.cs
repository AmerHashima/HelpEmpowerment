using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<PagedResponse<CourseDto>>> Search([FromBody] DataRequest request)
        {
            Guid? assignedUserId = null;
            var hasAuthorizationHeader = Request.Headers.ContainsKey("Authorization");

            if (hasAuthorizationHeader && User.Identity?.IsAuthenticated != true)
                return Unauthorized();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userType = User.FindFirstValue("UserType");
                if (string.Equals(userType, "Student", StringComparison.Ordinal))
                {
                    // A student token must not be interpreted as an internal user id.
                    // Student/public catalogue requests are not filtered by staff assignments.
                    var studentResponse = await _courseService.GetPagedAsync(request);
                    return studentResponse.Success ? Ok(studentResponse) : BadRequest(studentResponse);
                }

                if (!string.Equals(userType, "User", StringComparison.Ordinal))
                    return Unauthorized();

                if (!Guid.TryParse(
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"),
                        out var userId))
                    return Unauthorized();

                if (!User.IsInRole("Admin"))
                    assignedUserId = userId;
            }

            var response = await _courseService.GetPagedAsync(request, assignedUserId);
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
        [Authorize(Policy = "InternalUser")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Create([FromBody] CreateCourseDto dto)
        {
            var response = await _courseService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Policy = "InternalUser")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Update([FromBody] UpdateCourseDto dto)
        {
            var response = await _courseService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "InternalUser")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _courseService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
