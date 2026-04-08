using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
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

        [HttpPost("{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponse<CourseQuestionDto>>> UploadImage(Guid id, IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(ApiResponse<CourseQuestionDto>.ErrorResponse("No image file provided"));

            var response = await _questionService.UploadImageAsync(id, image);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var pathResponse = await _questionService.GetImagePathAsync(id);
            if (!pathResponse.Success)
                return NotFound(pathResponse);

            var filePath = pathResponse.Data!;
            if (!System.IO.File.Exists(filePath))
                return NotFound("Image file not found on server");

            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png"            => "image/png",
                ".gif"            => "image/gif",
                ".webp"           => "image/webp",
                _                 => "application/octet-stream"
            };

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, contentType);
        }
    }
}