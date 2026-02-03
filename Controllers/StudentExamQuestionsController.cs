using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentExamQuestionsController : ControllerBase
    {
        private readonly IStudentExamQuestionService _studentExamQuestionService;

        public StudentExamQuestionsController(IStudentExamQuestionService studentExamQuestionService)
        {
            _studentExamQuestionService = studentExamQuestionService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<StudentExamQuestionDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _studentExamQuestionService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentExamQuestionDto>>> GetById(Guid id)
        {
            var response = await _studentExamQuestionService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("exam/{studentExamId}")]
        public async Task<ActionResult<ApiResponse<List<StudentExamQuestionDto>>>> GetByStudentExamId(Guid studentExamId)
        {
            var response = await _studentExamQuestionService.GetByStudentExamIdAsync(studentExamId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudentExamQuestionDto>>> Create([FromBody] CreateStudentExamQuestionDto dto)
        {
            var response = await _studentExamQuestionService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<StudentExamQuestionDto>>> Update([FromBody] UpdateStudentExamQuestionDto dto)
        {
            var response = await _studentExamQuestionService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _studentExamQuestionService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}