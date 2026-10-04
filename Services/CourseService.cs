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
        private readonly IConfiguration _configuration;

        public CourseService(ICourseRepository courseRepository, IAppLookupDetailRepository lookupDetailRepository, IConfiguration configuration)
        {
            _courseRepository = courseRepository;
            _lookupDetailRepository = lookupDetailRepository;
            _configuration = configuration;
        }

        public async Task<PagedResponse<CourseDto>> GetPagedAsync(DataRequest request, Guid? assignedUserId = null)
        {
            try
            {
                var pagedResult = await _courseRepository.GetPagedAsync(request, assignedUserId);

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
                //if (!await _courseRepository.IsCourseCodeUniqueAsync(dto.CourseCode))
                //    return ApiResponse<CourseDto>.ErrorResponse("Course code already exists");

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
                    CertificateNumber = dto.CertificateNumber ?? 1,
                    CourseLevelLookupId = dto.CourseLevelLookupId,
                    CourseCategoryLookupId = dto.CourseCategoryLookupId,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    RecordedCourseReservPrice = dto.RecordedCourseReservPrice,
                    ExamSimulationReservPrice = dto.ExamSimulationReservPrice,
                    LiveCourseReservPrice = dto.LiveCourseReservPrice
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
                course.CertificateNumber = dto.CertificateNumber ?? 1;
                course.CourseLevelLookupId = dto.CourseLevelLookupId;
                course.CourseCategoryLookupId = dto.CourseCategoryLookupId;
                course.IsActive = dto.IsActive;
                course.UpdatedBy = dto.UpdatedBy;
                course.RecordedCourseReservPrice = dto.RecordedCourseReservPrice;
                course.ExamSimulationReservPrice = dto.ExamSimulationReservPrice;
                course.LiveCourseReservPrice = dto.LiveCourseReservPrice;

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

        private static readonly HashSet<string> AllowedImageExtensions = new() { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private string ImageStoragePath => _configuration["FileStorage:CourseImagesPath"] ?? "/var/www/images/courses";

        public async Task<ApiResponse<CourseDto>> UploadImageAsync(Guid id, IFormFile image)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null) return ApiResponse<CourseDto>.ErrorResponse("Course not found");
                if (image == null || image.Length == 0) return ApiResponse<CourseDto>.ErrorResponse("A non-empty image is required");
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (!AllowedImageExtensions.Contains(extension))
                    return ApiResponse<CourseDto>.ErrorResponse($"Invalid file type. Allowed: {string.Join(", ", AllowedImageExtensions)}");

                Directory.CreateDirectory(ImageStoragePath);
                var fileName = $"{id}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(ImageStoragePath, fileName);
                await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                    await image.CopyToAsync(stream);

                var oldFileName = course.ImagePath;
                course.ImagePath = fileName;
                var updated = await _courseRepository.UpdateAsync(course);
                DeletePhysicalImage(oldFileName);
                return ApiResponse<CourseDto>.SuccessResponse(MapToDto(updated), "Course image uploaded successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDto>.ErrorResponse($"Error uploading course image: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> GetImagePathAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return ApiResponse<string>.ErrorResponse("Course not found");
            return string.IsNullOrWhiteSpace(course.ImagePath)
                ? ApiResponse<string>.ErrorResponse("No image uploaded for this course")
                : ApiResponse<string>.SuccessResponse(course.ImagePath);
        }

        public async Task<ApiResponse<bool>> DeleteImageAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return ApiResponse<bool>.ErrorResponse("Course not found");
            if (string.IsNullOrWhiteSpace(course.ImagePath)) return ApiResponse<bool>.ErrorResponse("No image uploaded for this course");
            DeletePhysicalImage(course.ImagePath);
            course.ImagePath = null;
            await _courseRepository.UpdateAsync(course);
            return ApiResponse<bool>.SuccessResponse(true, "Course image deleted successfully");
        }

        private void DeletePhysicalImage(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var filePath = Path.Combine(ImageStoragePath, Path.GetFileName(fileName));
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        private static CourseDto MapToDto(Course course)
        {
            return new CourseDto
            {
                Oid = course.Oid,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                CourseDescription = course.CourseDescription,
                ImagePath = course.ImagePath,
                CertificateNumber = course.CertificateNumber ?? 1,
                CourseLevelLookupId = course.CourseLevelLookupId,
                CourseLevelName = course.CourseLevelLookup?.LookupNameEn,
                CourseCategoryLookupId = course.CourseCategoryLookupId,
                CourseCategoryName = course.CourseCategoryLookup?.LookupNameEn,
                RecordedCourseReservPrice = course.RecordedCourseReservPrice,
                ExamSimulationReservPrice = course.ExamSimulationReservPrice,
                LiveCourseReservPrice = course.LiveCourseReservPrice,
                IsActive = course.IsActive,
                CreatedAt = course.CreatedAt,
                CreatedBy = course.CreatedBy,
                UpdatedAt = course.UpdatedAt,
                UpdatedBy = course.UpdatedBy
            };
        }
    }
}
