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
        private readonly string videoPath = "/var/www/videos";

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
        [HttpGet("streamVideo/{*fileName}")]
        public IActionResult streamVideo(string fileName)
        {
            var pathResult = ResolveVideoPath(fileName);
            if (!pathResult.IsValid)
            {
                return BadRequest(new
                {
                    message = pathResult.Error,
                    requestedFileName = fileName,
                    decodedFileName = pathResult.DecodedFileName,
                    checkedPath = pathResult.FullPath,
                    videoBasePath = Path.GetFullPath(videoPath)
                });
            }

            if (!System.IO.File.Exists(pathResult.FullPath))
            {
                return NotFound(new
                {
                    message = "Video file not found on server",
                    requestedFileName = fileName,
                    decodedFileName = pathResult.DecodedFileName,
                    checkedPath = pathResult.FullPath,
                    videoBasePath = Path.GetFullPath(videoPath),
                    hint = "The URL contained %0D%0A, which is a newline. It is trimmed now, but the remaining file name/path must match a real file under the video folder."
                });
            }

            var stream = new FileStream(pathResult.FullPath!, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }

        [HttpGet("stream/{fileName}")]
        public IActionResult GetVideo(string fileName)
        {
            var pathResult = ResolveVideoPath(fileName);
            if (!pathResult.IsValid)
            {
                return BadRequest(new
                {
                    message = pathResult.Error,
                    requestedFileName = fileName,
                    decodedFileName = pathResult.DecodedFileName,
                    checkedPath = pathResult.FullPath,
                    videoBasePath = Path.GetFullPath(videoPath)
                });
            }

            if (!System.IO.File.Exists(pathResult.FullPath))
            {
                return NotFound(new
                {
                    message = "Video file not found on server",
                    requestedFileName = fileName,
                    decodedFileName = pathResult.DecodedFileName,
                    checkedPath = pathResult.FullPath,
                    videoBasePath = Path.GetFullPath(videoPath)
                });
            }

            var stream = new FileStream(pathResult.FullPath!, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }

        private VideoPathResult ResolveVideoPath(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return new VideoPathResult(false, null, null, "File name is required");

            var decodedFileName = Uri.UnescapeDataString(fileName).Trim();
            decodedFileName = decodedFileName.Replace('\\', Path.DirectorySeparatorChar)
                                             .Replace('/', Path.DirectorySeparatorChar);

            var basePath = Path.GetFullPath(videoPath);
            var fullPath = Path.IsPathRooted(decodedFileName)
                ? Path.GetFullPath(decodedFileName)
                : Path.GetFullPath(Path.Combine(basePath, decodedFileName));

            var isInsideVideoFolder = fullPath.Equals(basePath, StringComparison.OrdinalIgnoreCase)
                || fullPath.StartsWith(basePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

            if (!isInsideVideoFolder)
                return new VideoPathResult(false, decodedFileName, fullPath, "Invalid video path");

            return new VideoPathResult(true, decodedFileName, fullPath, null);
        }

        private sealed record VideoPathResult(bool IsValid, string? DecodedFileName, string? FullPath, string? Error);

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(UploadLimits.MaxVideoUploadSizeBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = UploadLimits.MaxVideoUploadSizeBytes)]
        public async Task<ActionResult<ApiResponse<CourseVideoDto>>> UploadVideo([FromForm] Guid courseVideoId, IFormFile video, [FromForm] string savePath)
        {
            if (video == null || video.Length == 0)
                return BadRequest(ApiResponse<CourseVideoDto>.ErrorResponse("No video file provided"));

            if (string.IsNullOrWhiteSpace(savePath))
                return BadRequest(ApiResponse<CourseVideoDto>.ErrorResponse("savePath is required"));

            var response = await _courseVideoService.UploadVideoAsync(courseVideoId, video, savePath);
            return response.Success ? Ok(response) : BadRequest(response);
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
