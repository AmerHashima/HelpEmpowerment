using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentCoursesController : ControllerBase
    {
        private readonly IStudentCourseService _service;

        public StudentCoursesController(IStudentCourseService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<StudentCourseDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _service.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentCourseDto>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<ApiResponse<List<StudentCourseDto>>>> GetByStudentId(Guid studentId)
        {
            var response = await _service.GetByStudentIdAsync(studentId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<StudentCourseDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _service.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("check/{studentId}/{courseId}")]
        public async Task<ActionResult<ApiResponse<bool>>> IsEnrolled(Guid studentId, Guid courseId)
        {
            var response = await _service.IsStudentEnrolledAsync(studentId, courseId);
            return Ok(response);
        }

        [HttpPost("enroll")]
        public async Task<ActionResult<ApiResponse<StudentCourseDto>>> Enroll([FromBody] CreateStudentCourseDto dto)
        {
            var response = await _service.EnrollStudentAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<StudentCourseDto>>> Update([FromBody] UpdateStudentCourseDto dto)
        {
            var response = await _service.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/payment")]
        public async Task<ActionResult<ApiResponse<StudentCourseDto>>> UpdatePayment(Guid id, [FromBody] UpdatePaymentRequest request)
        {
            var response = await _service.UpdatePaymentStatusAsync(id, request.PaymentStatusLookupId, request.TransactionId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/progress")]
        public async Task<ActionResult<ApiResponse<StudentCourseDto>>> UpdateProgress(Guid id, [FromBody] UpdateProgressRequest request)
        {
            var response = await _service.UpdateProgressAsync(id, request.CompletedLessons, request.TotalLessons);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _service.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }

    public class UpdatePaymentRequest
    {
        public Guid PaymentStatusLookupId { get; set; }
        public string? TransactionId { get; set; }
    }

    public class UpdateProgressRequest
    {
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
    }
}