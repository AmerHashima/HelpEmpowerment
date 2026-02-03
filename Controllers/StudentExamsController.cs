using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentExamsController : ControllerBase
    {
        private readonly IStudentExamService _studentExamService;

        public StudentExamsController(IStudentExamService studentExamService)
        {
            _studentExamService = studentExamService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<StudentExamDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _studentExamService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentExamDto>>> GetById(Guid id)
        {
            var response = await _studentExamService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<ApiResponse<List<StudentExamDto>>>> GetByStudentId(Guid studentId)
        {
            var response = await _studentExamService.GetByStudentIdAsync(studentId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("{id}/with-questions")]
        public async Task<ActionResult<ApiResponse<StudentExamDto>>> GetWithQuestions(Guid id)
        {
            var response = await _studentExamService.GetWithQuestionsAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("start")]
        public async Task<ActionResult<ApiResponse<StudentExamDto>>> StartExam([FromBody] CreateStudentExamDto dto)
        {
            var response = await _studentExamService.StartExamAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPost("submit")]
        public async Task<ActionResult<ApiResponse<StudentExamDto>>> SubmitExam([FromBody] SubmitStudentExamDto dto)
        {
            var response = await _studentExamService.SubmitExamAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _studentExamService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}