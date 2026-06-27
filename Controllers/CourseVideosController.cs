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
        private const string UploadPathPrefix = "/app/course-videos";
        private const string StreamRoutePrefix = "/api/CourseVideos/streamVideo/";

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
                    checkedPaths = pathResult.CheckedPaths,
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
                    checkedPaths = pathResult.CheckedPaths,
                    videoBasePath = Path.GetFullPath(videoPath),
                    uploadBasePath = Path.GetFullPath(UploadPathPrefix),
                    hint = "Use the VideoUrl returned from upload. The stream endpoint now checks the normal video folder and the upload folder prefix."
                });
            }

            var stream = new FileStream(pathResult.FullPath!, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }

        [HttpGet("{id}/stream")]
        public async Task<IActionResult> StreamById(Guid id)
        {
            var response = await _courseVideoService.GetByIdAsync(id);
            if (!response.Success || response.Data == null)
                return NotFound(response);

            if (string.IsNullOrWhiteSpace(response.Data.VideoUrl))
                return NotFound(new { message = "No video path saved for this course video", id });

            var pathResult = ResolveVideoPath(response.Data.VideoUrl, allowStoredAbsolutePath: true);
            if (!pathResult.IsValid)
            {
                return BadRequest(new
                {
                    message = pathResult.Error,
                    id,
                    videoUrl = response.Data.VideoUrl,
                    decodedFileName = pathResult.DecodedFileName,
                    checkedPath = pathResult.FullPath,
                    checkedPaths = pathResult.CheckedPaths,
                    videoBasePath = Path.GetFullPath(videoPath),
                    uploadBasePath = Path.GetFullPath(UploadPathPrefix)
                });
            }

            if (!System.IO.File.Exists(pathResult.FullPath))
            {
                return NotFound(new
                {
                    message = "Video file not found on server",
                    id,
                    videoUrl = response.Data.VideoUrl,
                    decodedFileName = pathResult.DecodedFileName,
                    checkedPath = pathResult.FullPath,
                    checkedPaths = pathResult.CheckedPaths,
                    videoBasePath = Path.GetFullPath(videoPath),
                    uploadBasePath = Path.GetFullPath(UploadPathPrefix)
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
                    checkedPaths = pathResult.CheckedPaths,
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
                    checkedPaths = pathResult.CheckedPaths,
                    videoBasePath = Path.GetFullPath(videoPath),
                    uploadBasePath = Path.GetFullPath(UploadPathPrefix)
                });
            }

            var stream = new FileStream(pathResult.FullPath!, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }

        private VideoPathResult ResolveVideoPath(string? fileName, bool allowStoredAbsolutePath = false)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return new VideoPathResult(false, null, null, Array.Empty<string>(), "File name is required");

            var decodedFileName = Uri.UnescapeDataString(fileName).Trim();
            decodedFileName = ExtractPathFromUrl(decodedFileName);
            decodedFileName = RemoveStreamRoutePrefix(decodedFileName);
            decodedFileName = decodedFileName.Replace('\\', Path.DirectorySeparatorChar)
                                             .Replace('/', Path.DirectorySeparatorChar);

            var basePath = Path.GetFullPath(videoPath);
            var uploadBasePath = Path.GetFullPath(UploadPathPrefix);
            var candidatePaths = new List<string>();

            if (Path.IsPathRooted(decodedFileName))
            {
                var rootedPath = Path.GetFullPath(decodedFileName);
                if (allowStoredAbsolutePath)
                    AddCandidate(candidatePaths, rootedPath);
                else
                    AddCandidate(candidatePaths, rootedPath, basePath, uploadBasePath);
            }
            else
            {
                AddCandidate(candidatePaths, Path.GetFullPath(Path.Combine(basePath, decodedFileName)), basePath, uploadBasePath);
                AddCandidate(candidatePaths, Path.GetFullPath(Path.Combine(uploadBasePath, decodedFileName)), basePath, uploadBasePath);
            }

            if (candidatePaths.Count == 0)
                return new VideoPathResult(false, decodedFileName, null, Array.Empty<string>(), "Invalid video path");

            var existingPath = candidatePaths.FirstOrDefault(System.IO.File.Exists);
            return new VideoPathResult(true, decodedFileName, existingPath ?? candidatePaths[0], candidatePaths, null);
        }

        private static string ExtractPathFromUrl(string value)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
                return value;

            return Uri.UnescapeDataString(uri.AbsolutePath);
        }

        private static string RemoveStreamRoutePrefix(string value)
        {
            if (value.StartsWith(StreamRoutePrefix, StringComparison.OrdinalIgnoreCase))
                return value.Substring(StreamRoutePrefix.Length);

            return value;
        }

        private static void AddCandidate(List<string> candidatePaths, string fullPath, params string[] allowedRoots)
        {
            if (allowedRoots.Length > 0 && !allowedRoots.Any(root => IsInsideRoot(fullPath, root)))
                return;

            if (!candidatePaths.Contains(fullPath, StringComparer.OrdinalIgnoreCase))
                candidatePaths.Add(fullPath);
        }

        private static bool IsInsideRoot(string fullPath, string rootPath)
        {
            return fullPath.Equals(rootPath, StringComparison.OrdinalIgnoreCase)
                || fullPath.StartsWith(rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        private sealed record VideoPathResult(
            bool IsValid,
            string? DecodedFileName,
            string? FullPath,
            IReadOnlyList<string> CheckedPaths,
            string? Error);

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
