using StandardArticture.Common;
using StandardArticture.DTOs;
using StandardArticture.IRepositories;
using StandardArticture.IServices;
using StandardArticture.Models;

namespace StandardArticture.Services
{
    public class CoursesMasterExamService : ICoursesMasterExamService
    {
        private readonly ICoursesMasterExamRepository _examRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;

        public CoursesMasterExamService(
            ICoursesMasterExamRepository examRepository, 
            ICourseRepository courseRepository,
            IAppLookupDetailRepository lookupDetailRepository)
        {
            _examRepository = examRepository;
            _courseRepository = courseRepository;
            _lookupDetailRepository = lookupDetailRepository;
        }

        public async Task<PagedResponse<CoursesMasterExamDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _examRepository.GetPagedAsync(request);

                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CoursesMasterExamDto>
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
                return new PagedResponse<CoursesMasterExamDto>
                {
                    Success = false,
                    Message = $"Error retrieving exams: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CoursesMasterExamDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var exam = await _examRepository.GetByIdAsync(id);
                if (exam == null)
                    return ApiResponse<CoursesMasterExamDto>.ErrorResponse("Exam not found");

                return ApiResponse<CoursesMasterExamDto>.SuccessResponse(MapToDto(exam));
            }
            catch (Exception ex)
            {
                return ApiResponse<CoursesMasterExamDto>.ErrorResponse($"Error retrieving exam: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CoursesMasterExamDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var exams = await _examRepository.GetByCourseIdAsync(courseId);
                var dtos = exams.Select(MapToDto).ToList();

                return ApiResponse<List<CoursesMasterExamDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CoursesMasterExamDto>>.ErrorResponse($"Error retrieving exams: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CoursesMasterExamDto>> GetWithQuestionsAsync(Guid id)
        {
            try
            {
                var exam = await _examRepository.GetWithQuestionsAsync(id);
                if (exam == null)
                    return ApiResponse<CoursesMasterExamDto>.ErrorResponse("Exam not found");

                return ApiResponse<CoursesMasterExamDto>.SuccessResponse(MapToDto(exam));
            }
            catch (Exception ex)
            {
                return ApiResponse<CoursesMasterExamDto>.ErrorResponse($"Error retrieving exam: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CoursesMasterExamDto>> CreateAsync(CreateCoursesMasterExamDto dto)
        {
            try
            {
                // Validate course exists
                var course = await _courseRepository.GetByIdAsync(dto.CourseOid);
                if (course == null)
                    return ApiResponse<CoursesMasterExamDto>.ErrorResponse("Course not found");

                // Validate Course Level Lookup if provided
                if (dto.CourseLevelLookupId.HasValue)
                {
                    var levelExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.CourseLevelLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!levelExists)
                        return ApiResponse<CoursesMasterExamDto>.ErrorResponse("Invalid Course Level. Please select a valid level.");
                }

                // Validate Course Category Lookup if provided
                if (dto.CourseCategoryLookupId.HasValue)
                {
                    var categoryExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.CourseCategoryLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!categoryExists)
                        return ApiResponse<CoursesMasterExamDto>.ErrorResponse("Invalid Course Category. Please select a valid category.");
                }

                var exam = new CoursesMasterExam
                {
                    CourseOid = dto.CourseOid,
                    CourseName = dto.CourseName,
                    CourseLevelLookupId = dto.CourseLevelLookupId,
                    CourseCategoryLookupId = dto.CourseCategoryLookupId,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy
                };

                var created = await _examRepository.AddAsync(exam);
                return ApiResponse<CoursesMasterExamDto>.SuccessResponse(MapToDto(created), "Exam created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CoursesMasterExamDto>.ErrorResponse($"Error creating exam: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CoursesMasterExamDto>> UpdateAsync(UpdateCoursesMasterExamDto dto)
        {
            try
            {
                var exam = await _examRepository.GetByIdAsync(dto.Oid);
                if (exam == null)
                    return ApiResponse<CoursesMasterExamDto>.ErrorResponse("Exam not found");

                // Validate Course Level Lookup if provided
                if (dto.CourseLevelLookupId.HasValue)
                {
                    var levelExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.CourseLevelLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!levelExists)
                        return ApiResponse<CoursesMasterExamDto>.ErrorResponse("Invalid Course Level. Please select a valid level.");
                }

                // Validate Course Category Lookup if provided
                if (dto.CourseCategoryLookupId.HasValue)
                {
                    var categoryExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.CourseCategoryLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!categoryExists)
                        return ApiResponse<CoursesMasterExamDto>.ErrorResponse("Invalid Course Category. Please select a valid category.");
                }

                exam.CourseOid = dto.CourseOid;
                exam.CourseName = dto.CourseName;
                exam.CourseLevelLookupId = dto.CourseLevelLookupId;
                exam.CourseCategoryLookupId = dto.CourseCategoryLookupId;
                exam.IsActive = dto.IsActive;
                exam.UpdatedBy = dto.UpdatedBy;

                var updated = await _examRepository.UpdateAsync(exam);
                return ApiResponse<CoursesMasterExamDto>.SuccessResponse(MapToDto(updated), "Exam updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CoursesMasterExamDto>.ErrorResponse($"Error updating exam: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _examRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Exam not found");

                return ApiResponse<bool>.SuccessResponse(true, "Exam deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting exam: {ex.Message}");
            }
        }

        private static CoursesMasterExamDto MapToDto(CoursesMasterExam exam)
        {
            return new CoursesMasterExamDto
            {
                Oid = exam.Oid,
                CourseOid = exam.CourseOid,
                CourseName = exam.CourseName,
                CourseCode = exam.Course?.CourseCode,
                CourseLevelLookupId = exam.CourseLevelLookupId,
                CourseLevelName = exam.CourseLevelLookup?.LookupNameEn,
                CourseCategoryLookupId = exam.CourseCategoryLookupId,
                CourseCategoryName = exam.CourseCategoryLookup?.LookupNameEn,
                IsActive = exam.IsActive,
                QuestionCount = exam.Questions?.Count ?? 0,
                CreatedAt = exam.CreatedAt,
                CreatedBy = exam.CreatedBy,
                UpdatedAt = exam.UpdatedAt,
                UpdatedBy = exam.UpdatedBy
            };
        }
    }
}