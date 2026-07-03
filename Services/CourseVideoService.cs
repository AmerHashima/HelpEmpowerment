using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using Microsoft.AspNetCore.Http;

namespace HelpEmpowermentApi.Services
{
    public class CourseVideoService : ICourseVideoService
    {
        private readonly ICourseVideoRepository _courseVideoRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;
        private readonly IConfiguration _configuration;

        public CourseVideoService(
            ICourseVideoRepository courseVideoRepository,
            ICourseRepository courseRepository,
            IAppLookupDetailRepository lookupDetailRepository,
            IConfiguration configuration)
        {
            _courseVideoRepository = courseVideoRepository;
            _courseRepository = courseRepository;
            _lookupDetailRepository = lookupDetailRepository;
            _configuration = configuration;
        }

        public async Task<PagedResponse<CourseVideoDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _courseVideoRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseVideoDto>
                {
                    Success = true,
                    Data = dtos,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                return new PagedResponse<CourseVideoDto>
                {
                    Success = false,
                    Message = $"Error retrieving course videos: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseVideoDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var video = await _courseVideoRepository.GetByIdAsync(id);
                if (video == null)
                    return ApiResponse<CourseVideoDto>.ErrorResponse("Course video not found");

                return ApiResponse<CourseVideoDto>.SuccessResponse(MapToDto(video));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseVideoDto>.ErrorResponse($"Error retrieving course video: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseVideoDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var videos = await _courseVideoRepository.GetByCourseIdAsync(courseId);
                var dtos = videos.Select(MapToDto).ToList();

                return ApiResponse<List<CourseVideoDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseVideoDto>>.ErrorResponse($"Error retrieving course videos: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseVideoDto>> GetWithAttachmentsAsync(Guid id)
        {
            try
            {
                var video = await _courseVideoRepository.GetWithAttachmentsAsync(id);
                if (video == null)
                    return ApiResponse<CourseVideoDto>.ErrorResponse("Course video not found");

                return ApiResponse<CourseVideoDto>.SuccessResponse(MapToDtoWithAttachments(video));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseVideoDto>.ErrorResponse($"Error retrieving course video: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseVideoDto>> CreateAsync(CreateCourseVideoDto dto)
        {
            try
            {
                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseVideoDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                // Validate Video Type Lookup if provided
                if (dto.VideoTypeLookupId.HasValue)
                {
                    var videoTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.VideoTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!videoTypeExists)
                        return ApiResponse<CourseVideoDto>.ErrorResponse("Invalid Video Type. Please select a valid type.");
                }

                var video = new CourseVideo
                {
                    CourseOid = dto.CourseOid,
                    NameEn = dto.NameEn,
                    NameAr = dto.NameAr,
                    VideoUrl = dto.VideoUrl,
                    DescriptionEn = dto.DescriptionEn,
                    DescriptionAr = dto.DescriptionAr,
                    DurationSeconds = dto.DurationSeconds,
                    OrderNo = dto.OrderNo,
                    VideoTypeLookupId = dto.VideoTypeLookupId,
                    IsPreview = dto.IsPreview,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                var createdVideo = await _courseVideoRepository.AddAsync(video);
                return ApiResponse<CourseVideoDto>.SuccessResponse(MapToDto(createdVideo), "Course video created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseVideoDto>.ErrorResponse($"Error creating course video: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseVideoDto>> UpdateAsync(UpdateCourseVideoDto dto)
        {
            try
            {
                var video = await _courseVideoRepository.GetByIdAsync(dto.Oid);
                if (video == null)
                    return ApiResponse<CourseVideoDto>.ErrorResponse("Course video not found");

                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseVideoDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                // Validate Video Type Lookup if provided
                if (dto.VideoTypeLookupId.HasValue)
                {
                    var videoTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.VideoTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!videoTypeExists)
                        return ApiResponse<CourseVideoDto>.ErrorResponse("Invalid Video Type. Please select a valid type.");
                }

                video.CourseOid = dto.CourseOid;
                video.NameEn = dto.NameEn;
                video.NameAr = dto.NameAr;
                video.VideoUrl = dto.VideoUrl;
                video.DescriptionEn = dto.DescriptionEn;
                video.DescriptionAr = dto.DescriptionAr;
                video.DurationSeconds = dto.DurationSeconds;
                video.OrderNo = dto.OrderNo;
                video.VideoTypeLookupId = dto.VideoTypeLookupId;
                video.IsPreview = dto.IsPreview;
                video.IsActive = dto.IsActive;
                video.UpdatedAt = DateTime.UtcNow;

                var updatedVideo = await _courseVideoRepository.UpdateAsync(video);
                return ApiResponse<CourseVideoDto>.SuccessResponse(MapToDto(updatedVideo), "Course video updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseVideoDto>.ErrorResponse($"Error updating course video: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseVideoDto>> UploadVideoAsync(Guid courseVideoId, IFormFile video, string? savePath)
        {
            try
            {
                var entity = await _courseVideoRepository.GetByIdAsync(courseVideoId);
                if (entity == null)
                    return ApiResponse<CourseVideoDto>.ErrorResponse("Course video not found");

                var ext = Path.GetExtension(video.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm" };
                if (!allowedExtensions.Contains(ext))
                    return ApiResponse<CourseVideoDto>.ErrorResponse($"Invalid video type. Allowed: {string.Join(", ", allowedExtensions)}");

                var fullSavePath = GetVideoStoragePath(savePath);
                Directory.CreateDirectory(fullSavePath);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(fullSavePath, fileName);

                if (!IsInsideRoot(Path.GetFullPath(filePath), fullSavePath))
                    return ApiResponse<CourseVideoDto>.ErrorResponse("Invalid video path");

                await using (var stream = new FileStream(filePath, FileMode.Create))
                    await video.CopyToAsync(stream);

                var normalizedPath = Path.GetRelativePath(fullSavePath, filePath)
                    .Replace("\\", "/");

                entity.VideoUrl = normalizedPath;
                entity.UpdatedAt = DateTime.UtcNow;

                var updated = await _courseVideoRepository.UpdateAsync(entity);
                return ApiResponse<CourseVideoDto>.SuccessResponse(MapToDto(updated), "Video uploaded and saved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseVideoDto>.ErrorResponse($"Error uploading video: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _courseVideoRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Course video not found");

                return ApiResponse<bool>.SuccessResponse(true, "Course video deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting course video: {ex.Message}");
            }
        }

        private CourseVideoDto MapToDto(CourseVideo video)
        {
            return new CourseVideoDto
            {
                Oid = video.Oid,
                CourseOid = video.CourseOid,
                CourseName = video.Course?.CourseName,
                NameEn = video.NameEn,
                NameAr = video.NameAr,
                VideoUrl = video.VideoUrl,
                DescriptionEn = video.DescriptionEn,
                DescriptionAr = video.DescriptionAr,
                DurationSeconds = video.DurationSeconds,
                OrderNo = video.OrderNo,
                VideoTypeLookupId = video.VideoTypeLookupId,
                VideoTypeName = video.VideoTypeLookup?.LookupNameEn,
                IsPreview = video.IsPreview,
                IsActive = video.IsActive,
                CreatedAt = video.CreatedAt
            };
        }

        private string GetVideoStoragePath(string? savePath)
        {
            var configuredPath = _configuration["FileStorage:VideosPath"];
            var storagePath = !string.IsNullOrWhiteSpace(configuredPath)
                ? configuredPath
                : !string.IsNullOrWhiteSpace(savePath)
                    ? savePath
                    : "/var/www/videos";

            return Path.GetFullPath(storagePath);
        }

        private static bool IsInsideRoot(string fullPath, string rootPath)
        {
            return fullPath.Equals(rootPath, StringComparison.OrdinalIgnoreCase)
                || fullPath.StartsWith(rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        private CourseVideoDto MapToDtoWithAttachments(CourseVideo video)
        {
            return new CourseVideoDto
            {
                Oid = video.Oid,
                CourseOid = video.CourseOid,
                CourseName = video.Course?.CourseName,
                NameEn = video.NameEn,
                NameAr = video.NameAr,
                VideoUrl = video.VideoUrl,
                DescriptionEn = video.DescriptionEn,
                DescriptionAr = video.DescriptionAr,
                DurationSeconds = video.DurationSeconds,
                OrderNo = video.OrderNo,
                VideoTypeLookupId = video.VideoTypeLookupId,
                VideoTypeName = video.VideoTypeLookup?.LookupNameEn,
                IsPreview = video.IsPreview,
                IsActive = video.IsActive,
                CreatedAt = video.CreatedAt,
                Attachments = video.Attachments.Select(a => new CourseVideoAttachmentDto
                {
                    Oid = a.Oid,
                    CourseVideoOid = a.CourseVideoOid,
                    FileName = a.FileName,
                    FileUrl = a.FileUrl,
                    FileTypeLookupId = a.FileTypeLookupId,
                    FileTypeName = a.FileTypeLookup?.LookupNameEn,
                    CreatedAt = a.CreatedAt
                }).ToList()
            };
        }
    }
}
