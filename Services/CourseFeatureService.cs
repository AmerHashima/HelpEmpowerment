using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseFeatureService : ICourseFeatureService
    {
        private readonly ICourseFeatureRepository _courseFeatureRepository;
        private readonly ICourseRepository _courseRepository;

        public CourseFeatureService(ICourseFeatureRepository courseFeatureRepository, ICourseRepository courseRepository)
        {
            _courseFeatureRepository = courseFeatureRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResponse<CourseFeatureDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _courseFeatureRepository.GetPagedAsync(request);

                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseFeatureDto>
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
                return new PagedResponse<CourseFeatureDto>
                {
                    Success = false,
                    Message = $"Error retrieving course features: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseFeatureDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var feature = await _courseFeatureRepository.GetByIdAsync(id);
                if (feature == null)
                    return ApiResponse<CourseFeatureDto>.ErrorResponse("Course feature not found");

                return ApiResponse<CourseFeatureDto>.SuccessResponse(MapToDto(feature));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseFeatureDto>.ErrorResponse($"Error retrieving course feature: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseFeatureDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var features = await _courseFeatureRepository.GetByCourseIdAsync(courseId);
                var dtos = features.Select(MapToDto).ToList();

                return ApiResponse<List<CourseFeatureDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseFeatureDto>>.ErrorResponse($"Error retrieving course features: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseFeatureDto>> CreateAsync(CreateCourseFeatureDto dto)
        {
            try
            {
                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseFeatureDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                var feature = new CourseFeature
                {
                    CourseOid = dto.CourseOid,
                    FeatureHeader = dto.FeatureHeader,
                    FeatureDescription = dto.FeatureDescription,
                    OrderNo = dto.OrderNo,
                    CreatedAt = DateTime.UtcNow
                };

                var createdFeature = await _courseFeatureRepository.AddAsync(feature);
                return ApiResponse<CourseFeatureDto>.SuccessResponse(MapToDto(createdFeature), "Course feature created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseFeatureDto>.ErrorResponse($"Error creating course feature: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseFeatureDto>> UpdateAsync(UpdateCourseFeatureDto dto)
        {
            try
            {
                var feature = await _courseFeatureRepository.GetByIdAsync(dto.Oid);
                if (feature == null)
                    return ApiResponse<CourseFeatureDto>.ErrorResponse("Course feature not found");

                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseFeatureDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                feature.CourseOid = dto.CourseOid;
                feature.FeatureHeader = dto.FeatureHeader;
                feature.FeatureDescription = dto.FeatureDescription;
                feature.OrderNo = dto.OrderNo;
                feature.UpdatedAt = DateTime.UtcNow;

                var updatedFeature = await _courseFeatureRepository.UpdateAsync(feature);
                return ApiResponse<CourseFeatureDto>.SuccessResponse(MapToDto(updatedFeature), "Course feature updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseFeatureDto>.ErrorResponse($"Error updating course feature: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _courseFeatureRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Course feature not found");

                return ApiResponse<bool>.SuccessResponse(true, "Course feature deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting course feature: {ex.Message}");
            }
        }

        private CourseFeatureDto MapToDto(CourseFeature feature)
        {
            return new CourseFeatureDto
            {
                Oid = feature.Oid,
                CourseOid = feature.CourseOid,
                CourseName = feature.Course?.CourseName,
                FeatureHeader = feature.FeatureHeader,
                FeatureDescription = feature.FeatureDescription,
                OrderNo = feature.OrderNo,
                CreatedAt = feature.CreatedAt
            };
        }
    }
}