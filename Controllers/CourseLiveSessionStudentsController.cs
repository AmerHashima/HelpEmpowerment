using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseLiveSessionStudentsController : ControllerBase
    {
        private readonly ICourseLiveSessionStudentService _sessionStudentService;

        public CourseLiveSessionStudentsController(ICourseLiveSessionStudentService sessionStudentService)
        {
            _sessionStudentService = sessionStudentService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseLiveSessionStudentDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _sessionStudentService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseLiveSessionStudentDto>>> GetById(Guid id)
        {
            var response = await _sessionStudentService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<ApiResponse<List<CourseLiveSessionStudentDto>>>> GetBySessionId(Guid sessionId)
        {
            var response = await _sessionStudentService.GetBySessionIdAsync(sessionId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<ApiResponse<List<CourseLiveSessionStudentDto>>>> GetByStudentId(Guid studentId)
        {
            var response = await _sessionStudentService.GetByStudentIdAsync(studentId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("enroll")]
        public async Task<ActionResult<ApiResponse<CourseLiveSessionStudentDto>>> EnrollStudent([FromBody] CreateCourseLiveSessionStudentDto dto)
        {
            var response = await _sessionStudentService.EnrollStudentAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPost("{id}/unenroll")]
        public async Task<ActionResult<ApiResponse<bool>>> UnenrollStudent(Guid id)
        {
            var response = await _sessionStudentService.UnenrollStudentAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _sessionStudentService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}