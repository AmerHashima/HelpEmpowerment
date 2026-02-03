using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseVideosController : ControllerBase
    {
        private readonly ICourseVideoService _courseVideoService;

        public CourseVideosController(ICourseVideoService courseVideoService)
        {
            _courseVideoService = courseVideoService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseVideoDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _courseVideoService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseVideoDto>>> GetById(Guid id)
        {
            var response = await _courseVideoService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CourseVideoDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _courseVideoService.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("{id}/with-attachments")]
        public async Task<ActionResult<ApiResponse<CourseVideoDto>>> GetWithAttachments(Guid id)
        {
            var response = await _courseVideoService.GetWithAttachmentsAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseVideoDto>>> Create([FromBody] CreateCourseVideoDto dto)
        {
            var response = await _courseVideoService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseVideoDto>>> Update([FromBody] UpdateCourseVideoDto dto)
        {
            var response = await _courseVideoService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _courseVideoService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}