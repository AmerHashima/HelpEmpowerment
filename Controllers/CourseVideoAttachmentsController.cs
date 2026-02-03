using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseVideoAttachmentsController : ControllerBase
    {
        private readonly ICourseVideoAttachmentService _attachmentService;

        public CourseVideoAttachmentsController(ICourseVideoAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseVideoAttachmentDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _attachmentService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseVideoAttachmentDto>>> GetById(Guid id)
        {
            var response = await _attachmentService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("video/{videoId}")]
        public async Task<ActionResult<ApiResponse<List<CourseVideoAttachmentDto>>>> GetByVideoId(Guid videoId)
        {
            var response = await _attachmentService.GetByVideoIdAsync(videoId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseVideoAttachmentDto>>> Create([FromBody] CreateCourseVideoAttachmentDto dto)
        {
            var response = await _attachmentService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseVideoAttachmentDto>>> Update([FromBody] UpdateCourseVideoAttachmentDto dto)
        {
            var response = await _attachmentService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _attachmentService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}