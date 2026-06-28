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
        private const string UploadPathPrefix = "/app/course-videos";
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

            // Local file path -> serve from disk
            var pathResult = ResolveAttachmentPath(fileUrl);
            if (!pathResult.IsValid)
            {
                return BadRequest(new
                {
                    message = pathResult.Error,
                    fileUrl,
                    decodedFileName = pathResult.DecodedFileName,
                    checkedPath = pathResult.FullPath,
                    checkedPaths = pathResult.CheckedPaths,
                    attachmentsBasePath = Path.GetFullPath(AttachmentsPath),
                    uploadBasePath = Path.GetFullPath(UploadPathPrefix)
                });
            }

            if (!System.IO.File.Exists(pathResult.FullPath))
            {
                return NotFound(new
                {
                    message = "File not found on server",
                    fileUrl,
                    decodedFileName = pathResult.DecodedFileName,
                    checkedPath = pathResult.FullPath,
                    checkedPaths = pathResult.CheckedPaths,
                    attachmentsBasePath = Path.GetFullPath(AttachmentsPath),
                    uploadBasePath = Path.GetFullPath(UploadPathPrefix),
                    hint = "Use the FileUrl returned from upload. The download endpoint now checks the normal attachments folder and the upload folder prefix."
                });
            }

            var stream = new FileStream(pathResult.FullPath!, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, contentType, fileName);
        }

        private AttachmentPathResult ResolveAttachmentPath(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return new AttachmentPathResult(false, null, null, Array.Empty<string>(), "File URL is required");

            var decodedFileName = Uri.UnescapeDataString(fileUrl).Trim()
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            var basePath = Path.GetFullPath(AttachmentsPath);
            var uploadBasePath = Path.GetFullPath(UploadPathPrefix);
            var candidatePaths = new List<string>();

            if (Path.IsPathRooted(decodedFileName))
            {
                AddCandidate(candidatePaths, Path.GetFullPath(decodedFileName), basePath, uploadBasePath);
            }
            else
            {
                AddCandidate(candidatePaths, Path.GetFullPath(Path.Combine(basePath, decodedFileName)), basePath, uploadBasePath);
                AddCandidate(candidatePaths, Path.GetFullPath(Path.Combine(uploadBasePath, decodedFileName)), basePath, uploadBasePath);
            }

            if (candidatePaths.Count == 0)
                return new AttachmentPathResult(false, decodedFileName, null, Array.Empty<string>(), "Invalid file path");

            var existingPath = candidatePaths.FirstOrDefault(System.IO.File.Exists);
            return new AttachmentPathResult(true, decodedFileName, existingPath ?? candidatePaths[0], candidatePaths, null);
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

        private sealed record AttachmentPathResult(
            bool IsValid,
            string? DecodedFileName,
            string? FullPath,
            IReadOnlyList<string> CheckedPaths,
            string? Error);

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(UploadLimits.MaxVideoUploadSizeBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = UploadLimits.MaxVideoUploadSizeBytes)]
        public async Task<ActionResult<ApiResponse<CourseVideoAttachmentDto>>> Upload(
            [FromForm] Guid courseVideoOid,
            IFormFile file,
            [FromForm] string savePath,
            [FromForm] Guid? fileTypeLookupId)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<CourseVideoAttachmentDto>.ErrorResponse("No file provided"));

            if (string.IsNullOrWhiteSpace(savePath))
                return BadRequest(ApiResponse<CourseVideoAttachmentDto>.ErrorResponse("savePath is required"));

            var response = await _attachmentService.UploadAsync(courseVideoOid, file, savePath, fileTypeLookupId);
            return response.Success ? Ok(response) : BadRequest(response);
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
