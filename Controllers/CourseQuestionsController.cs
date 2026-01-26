using Microsoft.AspNetCore.Mvc;
using StandardArticture.Common;
using StandardArticture.DTOs;
using StandardArticture.IServices;

namespace StandardArticture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseQuestionsController : ControllerBase
    {
        private readonly ICourseQuestionService _questionService;

        public CourseQuestionsController(ICourseQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseQuestionDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _questionService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseQuestionDto>>> GetById(Guid id)
        {
            var response = await _questionService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("exam/{examId}")]
        public async Task<ActionResult<ApiResponse<List<CourseQuestionDto>>>> GetByExamId(Guid examId)
        {
            var response = await _questionService.GetByExamIdAsync(examId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("{id}/with-answers")]
        public async Task<ActionResult<ApiResponse<CourseQuestionDto>>> GetWithAnswers(Guid id)
        {
            var response = await _questionService.GetWithAnswersAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("exam/{examId}/with-answers")]
        public async Task<ActionResult<ApiResponse<List<CourseQuestionDto>>>> GetWithAnswersByExamId(Guid examId)
        {
            var response = await _questionService.GetWithAnswersByExamIdAsync(examId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseQuestionDto>>> Create([FromBody] CreateCourseQuestionDto dto)
        {
            var response = await _questionService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseQuestionDto>>> Update([FromBody] UpdateCourseQuestionDto dto)
        {
            var response = await _questionService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _questionService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}