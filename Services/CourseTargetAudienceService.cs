using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseTargetAudienceService : ICourseTargetAudienceService
    {
        private readonly ICourseTargetAudienceRepository _targetAudienceRepository;
        private readonly ICourseRepository _courseRepository;

        public CourseTargetAudienceService(
            ICourseTargetAudienceRepository targetAudienceRepository,
            ICourseRepository courseRepository)
        {
            _targetAudienceRepository = targetAudienceRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResponse<CourseTargetAudienceDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _targetAudienceRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseTargetAudienceDto>
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
                return new PagedResponse<CourseTargetAudienceDto>
                {
                    Success = false,
                    Message = $"Error retrieving target audiences: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseTargetAudienceDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var targetAudience = await _targetAudienceRepository.GetByIdAsync(id);
                if (targetAudience == null)
                    return ApiResponse<CourseTargetAudienceDto>.ErrorResponse("Target audience not found");

                return ApiResponse<CourseTargetAudienceDto>.SuccessResponse(MapToDto(targetAudience));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseTargetAudienceDto>.ErrorResponse($"Error retrieving target audience: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseTargetAudienceDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var targetAudiences = await _targetAudienceRepository.GetByCourseIdAsync(courseId);
                var dtos = targetAudiences.Select(MapToDto).ToList();

                return ApiResponse<List<CourseTargetAudienceDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseTargetAudienceDto>>.ErrorResponse($"Error retrieving target audiences: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseTargetAudienceDto>> CreateAsync(CreateCourseTargetAudienceDto dto)
        {
            try
            {
                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseTargetAudienceDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                var targetAudience = new CourseTargetAudience
                {
                    CourseOid = dto.CourseOid,
                    DescriptionEn = dto.DescriptionEn,
                    DescriptionAr = dto.DescriptionAr,
                    OrderNo = dto.OrderNo,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdTargetAudience = await _targetAudienceRepository.AddAsync(targetAudience);
                return ApiResponse<CourseTargetAudienceDto>.SuccessResponse(MapToDto(createdTargetAudience), "Target audience created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseTargetAudienceDto>.ErrorResponse($"Error creating target audience: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseTargetAudienceDto>> UpdateAsync(UpdateCourseTargetAudienceDto dto)
        {
            try
            {
                var targetAudience = await _targetAudienceRepository.GetByIdAsync(dto.Oid);
                if (targetAudience == null)
                    return ApiResponse<CourseTargetAudienceDto>.ErrorResponse("Target audience not found");

                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseTargetAudienceDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                targetAudience.CourseOid = dto.CourseOid;
                targetAudience.DescriptionEn = dto.DescriptionEn;
                targetAudience.DescriptionAr = dto.DescriptionAr;
                targetAudience.OrderNo = dto.OrderNo;
                targetAudience.UpdatedBy = dto.UpdatedBy;
                targetAudience.UpdatedAt = DateTime.UtcNow;

                var updatedTargetAudience = await _targetAudienceRepository.UpdateAsync(targetAudience);
                return ApiResponse<CourseTargetAudienceDto>.SuccessResponse(MapToDto(updatedTargetAudience), "Target audience updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseTargetAudienceDto>.ErrorResponse($"Error updating target audience: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _targetAudienceRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Target audience not found");

                return ApiResponse<bool>.SuccessResponse(true, "Target audience deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting target audience: {ex.Message}");
            }
        }

        private CourseTargetAudienceDto MapToDto(CourseTargetAudience targetAudience)
        {
            return new CourseTargetAudienceDto
            {
                Oid = targetAudience.Oid,
                CourseOid = targetAudience.CourseOid,
                CourseName = targetAudience.Course?.CourseName,
                DescriptionEn = targetAudience.DescriptionEn,
                DescriptionAr = targetAudience.DescriptionAr,
                OrderNo = targetAudience.OrderNo,
                CreatedAt = targetAudience.CreatedAt,
                CreatedBy = targetAudience.CreatedBy,
                UpdatedAt = targetAudience.UpdatedAt,
                UpdatedBy = targetAudience.UpdatedBy
            };
        }
    }
}