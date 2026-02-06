using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseAnswersController : ControllerBase
    {
        private readonly ICourseAnswerService _answerService;

        public CourseAnswersController(ICourseAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseAnswerDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _answerService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseAnswerDto>>> GetById(Guid id)
        {
            var response = await _answerService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("question/{questionId}")]
        public async Task<ActionResult<ApiResponse<List<CourseAnswerDto>>>> GetByQuestionId(Guid questionId)
        {
            var response = await _answerService.GetByQuestionIdAsync(questionId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("question/{questionId}/correct")]
        public async Task<ActionResult<ApiResponse<CourseAnswerDto>>> GetCorrectAnswerByQuestionId(Guid questionId)
        {
            var response = await _answerService.GetCorrectAnswerByQuestionIdAsync(questionId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseAnswerDto>>> Create([FromBody] CreateCourseAnswerDto dto)
        {
            var response = await _answerService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseAnswerDto>>> Update([FromBody] UpdateCourseAnswerDto dto)
        {
            var response = await _answerService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _answerService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}