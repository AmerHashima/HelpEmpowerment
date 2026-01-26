using Microsoft.AspNetCore.Mvc;
using StandardArticture.Common;
using StandardArticture.DTOs;
using StandardArticture.IServices;

namespace StandardArticture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesMasterExamsController : ControllerBase
    {
        private readonly ICoursesMasterExamService _examService;

        public CoursesMasterExamsController(ICoursesMasterExamService examService)
        {
            _examService = examService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CoursesMasterExamDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _examService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CoursesMasterExamDto>>> GetById(Guid id)
        {
            var response = await _examService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CoursesMasterExamDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _examService.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("{id}/with-questions")]
        public async Task<ActionResult<ApiResponse<CoursesMasterExamDto>>> GetWithQuestions(Guid id)
        {
            var response = await _examService.GetWithQuestionsAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CoursesMasterExamDto>>> Create([FromBody] CreateCoursesMasterExamDto dto)
        {
            var response = await _examService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CoursesMasterExamDto>>> Update([FromBody] UpdateCoursesMasterExamDto dto)
        {
            var response = await _examService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _examService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}