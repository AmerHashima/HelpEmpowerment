using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using Microsoft.Extensions.Configuration;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseVideoAttachmentsController : ControllerBase
    {
        private readonly ICourseVideoAttachmentService _attachmentService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private string AttachmentsPath => _configuration["FileStorage:AttachmentsPath"] ?? "/var/www/attachments";

        public CourseVideoAttachmentsController(
            ICourseVideoAttachmentService attachmentService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _attachmentService = attachmentService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            var response = await _attachmentService.GetByIdAsync(id);
            if (!response.Success || response.Data == null)
                return NotFound("Attachment not found");

            var fileUrl = response.Data.FileUrl;
            if (string.IsNullOrWhiteSpace(fileUrl))
                return NotFound("No file URL associated with this attachment");

            var fileName = response.Data.FileName ?? Path.GetFileName(fileUrl) ?? $"{id}.pdf";
            const string contentType = "application/pdf";

            // Remote HTTP/HTTPS URL → proxy the download
            if (fileUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                fileUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var client = _httpClientFactory.CreateClient();
                var fileBytes = await client.GetByteArrayAsync(fileUrl);
                return File(fileBytes, contentType, fileName);
            }

            // Local file path → serve from disk
            var basePath = Path.GetFullPath(AttachmentsPath);
            var fullPath = Path.GetFullPath(Path.Combine(basePath, fileUrl));

            if (!fullPath.StartsWith(basePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && !fullPath.Equals(basePath, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file path");

            if (!System.IO.File.Exists(fullPath))
                return NotFound($"File not found on server: {fullPath}");

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, contentType, fileName);
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