using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseLiveSessionsController : ControllerBase
    {
        private readonly ICourseLiveSessionService _liveSessionService;

        public CourseLiveSessionsController(ICourseLiveSessionService liveSessionService)
        {
            _liveSessionService = liveSessionService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseLiveSessionDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _liveSessionService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseLiveSessionDto>>> GetById(Guid id)
        {
            var response = await _liveSessionService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CourseLiveSessionDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _liveSessionService.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("{id}/with-students")]
        public async Task<ActionResult<ApiResponse<CourseLiveSessionDto>>> GetWithStudents(Guid id)
        {
            var response = await _liveSessionService.GetWithStudentsAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}/upcoming")]
        public async Task<ActionResult<ApiResponse<List<CourseLiveSessionDto>>>> GetUpcomingSessions(Guid courseId)
        {
            var response = await _liveSessionService.GetUpcomingSessionsAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseLiveSessionDto>>> Create([FromBody] CreateCourseLiveSessionDto dto)
        {
            var response = await _liveSessionService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseLiveSessionDto>>> Update([FromBody] UpdateCourseLiveSessionDto dto)
        {
            var response = await _liveSessionService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _liveSessionService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}