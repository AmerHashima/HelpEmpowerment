using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseContentService : ICourseContentService
    {
        private readonly ICourseContentRepository _courseContentRepository;
        private readonly ICourseOutlineRepository _courseOutlineRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;

        public CourseContentService(
            ICourseContentRepository courseContentRepository,
            ICourseOutlineRepository courseOutlineRepository,
            IAppLookupDetailRepository lookupDetailRepository)
        {
            _courseContentRepository = courseContentRepository;
            _courseOutlineRepository = courseOutlineRepository;
            _lookupDetailRepository = lookupDetailRepository;
        }

        public async Task<PagedResponse<CourseContentDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _courseContentRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseContentDto>
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
                return new PagedResponse<CourseContentDto>
                {
                    Success = false,
                    Message = $"Error retrieving course contents: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseContentDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var content = await _courseContentRepository.GetByIdAsync(id);
                if (content == null)
                    return ApiResponse<CourseContentDto>.ErrorResponse("Course content not found");

                return ApiResponse<CourseContentDto>.SuccessResponse(MapToDto(content));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseContentDto>.ErrorResponse($"Error retrieving course content: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseContentDto>>> GetByOutlineIdAsync(Guid outlineId)
        {
            try
            {
                var contents = await _courseContentRepository.GetByOutlineIdAsync(outlineId);
                var dtos = contents.Select(MapToDto).ToList();

                return ApiResponse<List<CourseContentDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseContentDto>>.ErrorResponse($"Error retrieving course contents: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseContentDto>> CreateAsync(CreateCourseContentDto dto)
        {
            try
            {
                // Validate Course Outline exists
                var outlineExists = await _courseOutlineRepository.ExistsAsync(o => o.Oid == dto.CourseOutlineOid && !o.IsDeleted);
                if (!outlineExists)
                    return ApiResponse<CourseContentDto>.ErrorResponse("Invalid Course Outline. Please select a valid outline.");

                // Validate Content Type Lookup if provided
                if (dto.ContentTypeLookupId.HasValue)
                {
                    var contentTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.ContentTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!contentTypeExists)
                        return ApiResponse<CourseContentDto>.ErrorResponse("Invalid Content Type. Please select a valid type.");
                }

                var content = new CourseContent
                {
                    CourseOutlineOid = dto.CourseOutlineOid,
                    TitleEn = dto.TitleEn,
                    TitleAr = dto.TitleAr,
                    ContentTypeLookupId = dto.ContentTypeLookupId,
                    ContentOid = dto.ContentOid,
                    OrderNo = dto.OrderNo,
                    IsFree = dto.IsFree,
                    CreatedAt = DateTime.UtcNow
                };

                var createdContent = await _courseContentRepository.AddAsync(content);
                return ApiResponse<CourseContentDto>.SuccessResponse(MapToDto(createdContent), "Course content created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseContentDto>.ErrorResponse($"Error creating course content: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseContentDto>> UpdateAsync(UpdateCourseContentDto dto)
        {
            try
            {
                var content = await _courseContentRepository.GetByIdAsync(dto.Oid);
                if (content == null)
                    return ApiResponse<CourseContentDto>.ErrorResponse("Course content not found");

                // Validate Course Outline exists
                var outlineExists = await _courseOutlineRepository.ExistsAsync(o => o.Oid == dto.CourseOutlineOid && !o.IsDeleted);
                if (!outlineExists)
                    return ApiResponse<CourseContentDto>.ErrorResponse("Invalid Course Outline. Please select a valid outline.");

                // Validate Content Type Lookup if provided
                if (dto.ContentTypeLookupId.HasValue)
                {
                    var contentTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.ContentTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!contentTypeExists)
                        return ApiResponse<CourseContentDto>.ErrorResponse("Invalid Content Type. Please select a valid type.");
                }

                content.CourseOutlineOid = dto.CourseOutlineOid;
                content.TitleEn = dto.TitleEn;
                content.TitleAr = dto.TitleAr;
                content.ContentTypeLookupId = dto.ContentTypeLookupId;
                content.ContentOid = dto.ContentOid;
                content.OrderNo = dto.OrderNo;
                content.IsFree = dto.IsFree;
                content.UpdatedAt = DateTime.UtcNow;

                var updatedContent = await _courseContentRepository.UpdateAsync(content);
                return ApiResponse<CourseContentDto>.SuccessResponse(MapToDto(updatedContent), "Course content updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseContentDto>.ErrorResponse($"Error updating course content: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _courseContentRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Course content not found");

                return ApiResponse<bool>.SuccessResponse(true, "Course content deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting course content: {ex.Message}");
            }
        }

        private CourseContentDto MapToDto(CourseContent content)
        {
            return new CourseContentDto
            {
                Oid = content.Oid,
                CourseOutlineOid = content.CourseOutlineOid,
                OutlineTitle = content.CourseOutline?.TitleEn,
                TitleEn = content.TitleEn,
                TitleAr = content.TitleAr,
                ContentTypeLookupId = content.ContentTypeLookupId,
                ContentTypeName = content.ContentTypeLookup?.LookupNameEn,
                ContentOid = content.ContentOid,
                OrderNo = content.OrderNo,
                IsFree = content.IsFree,
                CreatedAt = content.CreatedAt
            };
        }
    }
}