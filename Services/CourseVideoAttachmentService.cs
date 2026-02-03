using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseVideoAttachmentService : ICourseVideoAttachmentService
    {
        private readonly ICourseVideoAttachmentRepository _attachmentRepository;
        private readonly ICourseVideoRepository _courseVideoRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;

        public CourseVideoAttachmentService(
            ICourseVideoAttachmentRepository attachmentRepository,
            ICourseVideoRepository courseVideoRepository,
            IAppLookupDetailRepository lookupDetailRepository)
        {
            _attachmentRepository = attachmentRepository;
            _courseVideoRepository = courseVideoRepository;
            _lookupDetailRepository = lookupDetailRepository;
        }

        public async Task<PagedResponse<CourseVideoAttachmentDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _attachmentRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseVideoAttachmentDto>
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
                return new PagedResponse<CourseVideoAttachmentDto>
                {
                    Success = false,
                    Message = $"Error retrieving video attachments: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseVideoAttachmentDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var attachment = await _attachmentRepository.GetByIdAsync(id);
                if (attachment == null)
                    return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse("Video attachment not found");

                return ApiResponse<CourseVideoAttachmentDto>.SuccessResponse(MapToDto(attachment));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse($"Error retrieving video attachment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseVideoAttachmentDto>>> GetByVideoIdAsync(Guid videoId)
        {
            try
            {
                var attachments = await _attachmentRepository.GetByVideoIdAsync(videoId);
                var dtos = attachments.Select(MapToDto).ToList();

                return ApiResponse<List<CourseVideoAttachmentDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseVideoAttachmentDto>>.ErrorResponse($"Error retrieving video attachments: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseVideoAttachmentDto>> CreateAsync(CreateCourseVideoAttachmentDto dto)
        {
            try
            {
                // Validate Course Video exists
                var videoExists = await _courseVideoRepository.ExistsAsync(v => v.Oid == dto.CourseVideoOid && !v.IsDeleted);
                if (!videoExists)
                    return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse("Invalid Course Video. Please select a valid video.");

                // Validate File Type Lookup if provided
                if (dto.FileTypeLookupId.HasValue)
                {
                    var fileTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.FileTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!fileTypeExists)
                        return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse("Invalid File Type. Please select a valid type.");
                }

                var attachment = new CourseVideoAttachment
                {
                    CourseVideoOid = dto.CourseVideoOid,
                    FileName = dto.FileName,
                    FileUrl = dto.FileUrl,
                    FileTypeLookupId = dto.FileTypeLookupId,
                    CreatedAt = DateTime.UtcNow
                };

                var createdAttachment = await _attachmentRepository.AddAsync(attachment);
                return ApiResponse<CourseVideoAttachmentDto>.SuccessResponse(MapToDto(createdAttachment), "Video attachment created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse($"Error creating video attachment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseVideoAttachmentDto>> UpdateAsync(UpdateCourseVideoAttachmentDto dto)
        {
            try
            {
                var attachment = await _attachmentRepository.GetByIdAsync(dto.Oid);
                if (attachment == null)
                    return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse("Video attachment not found");

                // Validate Course Video exists
                var videoExists = await _courseVideoRepository.ExistsAsync(v => v.Oid == dto.CourseVideoOid && !v.IsDeleted);
                if (!videoExists)
                    return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse("Invalid Course Video. Please select a valid video.");

                // Validate File Type Lookup if provided
                if (dto.FileTypeLookupId.HasValue)
                {
                    var fileTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.FileTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!fileTypeExists)
                        return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse("Invalid File Type. Please select a valid type.");
                }

                attachment.CourseVideoOid = dto.CourseVideoOid;
                attachment.FileName = dto.FileName;
                attachment.FileUrl = dto.FileUrl;
                attachment.FileTypeLookupId = dto.FileTypeLookupId;
                attachment.UpdatedAt = DateTime.UtcNow;

                var updatedAttachment = await _attachmentRepository.UpdateAsync(attachment);
                return ApiResponse<CourseVideoAttachmentDto>.SuccessResponse(MapToDto(updatedAttachment), "Video attachment updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseVideoAttachmentDto>.ErrorResponse($"Error updating video attachment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _attachmentRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Video attachment not found");

                return ApiResponse<bool>.SuccessResponse(true, "Video attachment deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting video attachment: {ex.Message}");
            }
        }

        private CourseVideoAttachmentDto MapToDto(CourseVideoAttachment attachment)
        {
            return new CourseVideoAttachmentDto
            {
                Oid = attachment.Oid,
                CourseVideoOid = attachment.CourseVideoOid,
                VideoName = attachment.CourseVideo?.NameEn,
                FileName = attachment.FileName,
                FileUrl = attachment.FileUrl,
                FileTypeLookupId = attachment.FileTypeLookupId,
                FileTypeName = attachment.FileTypeLookup?.LookupNameEn,
                CreatedAt = attachment.CreatedAt
            };
        }
    }
}