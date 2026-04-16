using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;
using Microsoft.Extensions.Configuration;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseVideosController : ControllerBase
    {
        private readonly ICourseVideoService _courseVideoService;
        private readonly IConfiguration _configuration;
        private string VideosPath => _configuration["FileStorage:VideosPath"] ?? "/var/www/videos";

        public CourseVideosController(ICourseVideoService courseVideoService, IConfiguration configuration)
        {
            _courseVideoService = courseVideoService;
            _configuration = configuration;
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

        [HttpGet("streamVideo")]
        public IActionResult streamVideo([FromQuery] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("fileName query parameter is required");

            var basePath = Path.GetFullPath(VideosPath);
            var fullPath = Path.GetFullPath(Path.Combine(basePath, fileName));

            if (!fullPath.StartsWith(basePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && !fullPath.Equals(basePath, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file path");

            if (!System.IO.File.Exists(fullPath))
                return NotFound($"Video not found: {fileName}");

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }

        [HttpGet("stream/{fileName}")]
        public IActionResult GetVideo(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            var path = Path.Combine(VideosPath, fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
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