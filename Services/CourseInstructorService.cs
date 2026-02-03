using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseInstructorService : ICourseInstructorService
    {
        private readonly ICourseInstructorRepository _instructorRepository;
        private readonly ICourseRepository _courseRepository;

        public CourseInstructorService(
            ICourseInstructorRepository instructorRepository,
            ICourseRepository courseRepository)
        {
            _instructorRepository = instructorRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResponse<CourseInstructorDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _instructorRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseInstructorDto>
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
                return new PagedResponse<CourseInstructorDto>
                {
                    Success = false,
                    Message = $"Error retrieving course instructors: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseInstructorDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var instructor = await _instructorRepository.GetByIdAsync(id);
                if (instructor == null)
                    return ApiResponse<CourseInstructorDto>.ErrorResponse("Course instructor not found");

                return ApiResponse<CourseInstructorDto>.SuccessResponse(MapToDto(instructor));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseInstructorDto>.ErrorResponse($"Error retrieving course instructor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseInstructorDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var instructors = await _instructorRepository.GetByCourseIdAsync(courseId);
                var dtos = instructors.Select(MapToDto).ToList();

                return ApiResponse<List<CourseInstructorDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseInstructorDto>>.ErrorResponse($"Error retrieving course instructors: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseInstructorDto>> CreateAsync(CreateCourseInstructorDto dto)
        {
            try
            {
                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseInstructorDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                var instructor = new CourseInstructor
                {
                    CourseOid = dto.CourseOid,
                    HeaderAr = dto.HeaderAr,
                    HeaderEn = dto.HeaderEn,
                    BioEn = dto.BioEn,
                    BioAr = dto.BioAr,
                    ExperienceYears = dto.ExperienceYears,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdInstructor = await _instructorRepository.AddAsync(instructor);
                return ApiResponse<CourseInstructorDto>.SuccessResponse(MapToDto(createdInstructor), "Course instructor created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseInstructorDto>.ErrorResponse($"Error creating course instructor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseInstructorDto>> UpdateAsync(UpdateCourseInstructorDto dto)
        {
            try
            {
                var instructor = await _instructorRepository.GetByIdAsync(dto.Oid);
                if (instructor == null)
                    return ApiResponse<CourseInstructorDto>.ErrorResponse("Course instructor not found");

                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseInstructorDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                instructor.CourseOid = dto.CourseOid;
                instructor.HeaderAr = dto.HeaderAr;
                instructor.HeaderEn = dto.HeaderEn;
                instructor.BioEn = dto.BioEn;
                instructor.BioAr = dto.BioAr;
                instructor.ExperienceYears = dto.ExperienceYears;
                instructor.UpdatedBy = dto.UpdatedBy;
                instructor.UpdatedAt = DateTime.UtcNow;

                var updatedInstructor = await _instructorRepository.UpdateAsync(instructor);
                return ApiResponse<CourseInstructorDto>.SuccessResponse(MapToDto(updatedInstructor), "Course instructor updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseInstructorDto>.ErrorResponse($"Error updating course instructor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _instructorRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Course instructor not found");

                return ApiResponse<bool>.SuccessResponse(true, "Course instructor deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting course instructor: {ex.Message}");
            }
        }

        private CourseInstructorDto MapToDto(CourseInstructor instructor)
        {
            return new CourseInstructorDto
            {
                Oid = instructor.Oid,
                CourseOid = instructor.CourseOid,
                CourseName = instructor.Course?.CourseName,
                HeaderAr = instructor.HeaderAr,
                HeaderEn = instructor.HeaderEn,
                BioEn = instructor.BioEn,
                BioAr = instructor.BioAr,
                ExperienceYears = instructor.ExperienceYears,
                CreatedAt = instructor.CreatedAt,
                CreatedBy = instructor.CreatedBy,
                UpdatedAt = instructor.UpdatedAt,
                UpdatedBy = instructor.UpdatedBy
            };
        }
    }
}