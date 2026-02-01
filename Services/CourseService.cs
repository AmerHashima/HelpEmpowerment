using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;

        public CourseService(ICourseRepository courseRepository, IAppLookupDetailRepository lookupDetailRepository)
        {
            _courseRepository = courseRepository;
            _lookupDetailRepository = lookupDetailRepository;
        }

        public async Task<PagedResponse<CourseDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _courseRepository.GetPagedAsync(request);

                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseDto>
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
                return new PagedResponse<CourseDto>
                {
                    Success = false,
                    Message = $"Error retrieving courses: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null)
                    return ApiResponse<CourseDto>.ErrorResponse("Course not found");

                return ApiResponse<CourseDto>.SuccessResponse(MapToDto(course));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDto>.ErrorResponse($"Error retrieving course: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseDto>> GetByCodeAsync(string courseCode)
        {
            try
            {
                var course = await _courseRepository.GetByCodeAsync(courseCode);
                if (course == null)
                    return ApiResponse<CourseDto>.ErrorResponse("Course not found");

                return ApiResponse<CourseDto>.SuccessResponse(MapToDto(course));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDto>.ErrorResponse($"Error retrieving course: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseDto>> CreateAsync(CreateCourseDto dto)
        {
            try
            {
                // Validate unique course code
                if (!await _courseRepository.IsCourseCodeUniqueAsync(dto.CourseCode))
                    return ApiResponse<CourseDto>.ErrorResponse("Course code already exists");

                // Validate Course Level Lookup if provided
                if (dto.CourseLevelLookupId.HasValue)
                {
                    var levelExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.CourseLevelLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!levelExists)
                        return ApiResponse<CourseDto>.ErrorResponse("Invalid Course Level. Please select a valid level.");
                }

                // Validate Course Category Lookup if provided
                if (dto.CourseCategoryLookupId.HasValue)
                {
                    var categoryExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.CourseCategoryLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!categoryExists)
                        return ApiResponse<CourseDto>.ErrorResponse("Invalid Course Category. Please select a valid category.");
                }

                var course = new Course
                {
                    CourseCode = dto.CourseCode,
                    CourseName = dto.CourseName,
                    CourseDescription = dto.CourseDescription,
                    CourseLevelLookupId = dto.CourseLevelLookupId,
                    CourseCategoryLookupId = dto.CourseCategoryLookupId,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy
                };

                var created = await _courseRepository.AddAsync(course);
                return ApiResponse<CourseDto>.SuccessResponse(MapToDto(created), "Course created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDto>.ErrorResponse($"Error creating course: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseDto>> UpdateAsync(UpdateCourseDto dto)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(dto.Oid);
                if (course == null)
                    return ApiResponse<CourseDto>.ErrorResponse("Course not found");

                // Validate unique course code
                if (!await _courseRepository.IsCourseCodeUniqueAsync(dto.CourseCode, dto.Oid))
                    return ApiResponse<CourseDto>.ErrorResponse("Course code already exists");

                // Validate Course Level Lookup if provided
                if (dto.CourseLevelLookupId.HasValue)
                {
                    var levelExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.CourseLevelLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!levelExists)
                        return ApiResponse<CourseDto>.ErrorResponse("Invalid Course Level. Please select a valid level.");
                }

                // Validate Course Category Lookup if provided
                if (dto.CourseCategoryLookupId.HasValue)
                {
                    var categoryExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.CourseCategoryLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!categoryExists)
                        return ApiResponse<CourseDto>.ErrorResponse("Invalid Course Category. Please select a valid category.");
                }

                course.CourseCode = dto.CourseCode;
                course.CourseName = dto.CourseName;
                course.CourseDescription = dto.CourseDescription;
                course.CourseLevelLookupId = dto.CourseLevelLookupId;
                course.CourseCategoryLookupId = dto.CourseCategoryLookupId;
                course.IsActive = dto.IsActive;
                course.UpdatedBy = dto.UpdatedBy;

                var updated = await _courseRepository.UpdateAsync(course);
                return ApiResponse<CourseDto>.SuccessResponse(MapToDto(updated), "Course updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDto>.ErrorResponse($"Error updating course: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _courseRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Course not found");

                return ApiResponse<bool>.SuccessResponse(true, "Course deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting course: {ex.Message}");
            }
        }

        private static CourseDto MapToDto(Course course)
        {
            return new CourseDto
            {
                Oid = course.Oid,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                CourseDescription = course.CourseDescription,
                CourseLevelLookupId = course.CourseLevelLookupId,
                CourseLevelName = course.CourseLevelLookup?.LookupNameEn,
                CourseCategoryLookupId = course.CourseCategoryLookupId,
                CourseCategoryName = course.CourseCategoryLookup?.LookupNameEn,
                IsActive = course.IsActive,
                CreatedAt = course.CreatedAt,
                CreatedBy = course.CreatedBy,
                UpdatedAt = course.UpdatedAt,
                UpdatedBy = course.UpdatedBy
            };
        }
    }
}