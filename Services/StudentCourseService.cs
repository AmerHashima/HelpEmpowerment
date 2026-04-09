using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class StudentCourseService : IStudentCourseService
    {
        private readonly IStudentCourseRepository _repository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseVideoRepository _courseVideoRepository;

        public StudentCourseService(
            IStudentCourseRepository repository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            ICourseVideoRepository courseVideoRepository)
        {
            _repository = repository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _courseVideoRepository = courseVideoRepository;
        }

        public async Task<PagedResponse<StudentCourseDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _repository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<StudentCourseDto>
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
                return new PagedResponse<StudentCourseDto>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<StudentCourseDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetWithDetailsAsync(id);
                if (entity == null)
                    return ApiResponse<StudentCourseDto>.ErrorResponse("Enrollment not found");

                return ApiResponse<StudentCourseDto>.SuccessResponse(MapToDto(entity));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentCourseDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<StudentCourseDto>>> GetByStudentIdAsync(Guid studentId)
        {
            try
            {
                var entities = await _repository.GetByStudentIdAsync(studentId);
                var dtos = entities.Select(MapToDto).ToList();
                return ApiResponse<List<StudentCourseDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<StudentCourseDto>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<StudentCourseDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var entities = await _repository.GetByCourseIdAsync(courseId);
                var dtos = entities.Select(MapToDto).ToList();
                return ApiResponse<List<StudentCourseDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<StudentCourseDto>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentCourseDto>> EnrollStudentAsync(CreateStudentCourseDto dto)
        {
            try
            {
                // Check if student exists
                var student = await _studentRepository.GetByIdAsync(dto.StudentId);
                if (student == null)
                    return ApiResponse<StudentCourseDto>.ErrorResponse("Student not found");

                // Check if course exists
                var course = await _courseRepository.GetByIdAsync(dto.CourseId);
                if (course == null)
                    return ApiResponse<StudentCourseDto>.ErrorResponse("Course not found");

                // Check if already enrolled
                var existingEnrollment = await _repository.GetByStudentAndCourseAsync(dto.StudentId, dto.CourseId);
                if (existingEnrollment != null)
                    return ApiResponse<StudentCourseDto>.ErrorResponse("Student is already enrolled in this course");

                // ✅ FIX 2: Parse course.Price from string to decimal
                decimal? coursePrice = null;
                if (!string.IsNullOrEmpty(course.Price) && decimal.TryParse(course.Price, out var parsedPrice))
                {
                    coursePrice = parsedPrice;
                }

                var totalLessons = (await _courseVideoRepository.GetByCourseIdAsync(dto.CourseId)).Count;

                var entity = new StudentCourse
                {
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId,
                    Price = dto.Price ?? coursePrice,  // ✅ Fixed: Now both are decimal?
                    DiscountAmount = dto.DiscountAmount,
                    PaymentMethod = dto.PaymentMethod,
                    EnrollmentDate = DateTime.UtcNow,
                    TotalLessons = totalLessons,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(entity);
                var result = await _repository.GetWithDetailsAsync(created.Oid);
                return ApiResponse<StudentCourseDto>.SuccessResponse(MapToDto(result!), "Student enrolled successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentCourseDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentCourseDto>> UpdateAsync(UpdateStudentCourseDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(dto.Oid);
                if (entity == null)
                    return ApiResponse<StudentCourseDto>.ErrorResponse("Enrollment not found");

                var totalLessons = (await _courseVideoRepository.GetByCourseIdAsync(entity.CourseId)).Count;

                entity.PaymentStatusLookupId = dto.PaymentStatusLookupId;
                entity.PaidAmount = dto.PaidAmount;
                entity.TransactionId = dto.TransactionId;
                entity.PaymentDate = dto.PaymentDate;
                entity.EnrollmentStatusLookupId = dto.EnrollmentStatusLookupId;
                entity.ProgressPercentage = dto.ProgressPercentage;
                entity.CompletedLessons = dto.CompletedLessons;
                entity.TotalLessons = totalLessons;
                entity.UpdatedBy = dto.UpdatedBy;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                var result = await _repository.GetWithDetailsAsync(dto.Oid);
                return ApiResponse<StudentCourseDto>.SuccessResponse(MapToDto(result!), "Enrollment updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentCourseDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentCourseDto>> UpdatePaymentStatusAsync(Guid id, Guid paymentStatusLookupId, string? transactionId)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<StudentCourseDto>.ErrorResponse("Enrollment not found");

                entity.PaymentStatusLookupId = paymentStatusLookupId;
                entity.TransactionId = transactionId;
                entity.PaymentDate = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                var result = await _repository.GetWithDetailsAsync(id);
                return ApiResponse<StudentCourseDto>.SuccessResponse(MapToDto(result!));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentCourseDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentCourseDto>> UpdateProgressAsync(Guid id, int completedLessons, int totalLessons)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return ApiResponse<StudentCourseDto>.ErrorResponse("Enrollment not found");

                entity.CompletedLessons = completedLessons;
                entity.TotalLessons = totalLessons;
                entity.ProgressPercentage = totalLessons > 0 ? (int)((completedLessons / (double)totalLessons) * 100) : 0;
                entity.UpdatedAt = DateTime.UtcNow;

                if (entity.ProgressPercentage >= 100)
                {
                    entity.CompletedDate = DateTime.UtcNow;
                }

                await _repository.UpdateAsync(entity);
                var result = await _repository.GetWithDetailsAsync(id);
                return ApiResponse<StudentCourseDto>.SuccessResponse(MapToDto(result!));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentCourseDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> IsStudentEnrolledAsync(Guid studentId, Guid courseId)
        {
            try
            {
                var isEnrolled = await _repository.IsStudentEnrolledAsync(studentId, courseId);
                return ApiResponse<bool>.SuccessResponse(isEnrolled);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Enrollment not found");

                return ApiResponse<bool>.SuccessResponse(true, "Enrollment deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static StudentCourseDto MapToDto(StudentCourse entity)
        {
            return new StudentCourseDto
            {
                Oid = entity.Oid,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.NameEn,
                CourseId = entity.CourseId,
                CourseName = entity.Course?.CourseName,
                PaymentStatusLookupId = entity.PaymentStatusLookupId,
                PaymentStatusName = entity.PaymentStatus?.LookupNameEn,
                Price = entity.Price,
                DiscountAmount = entity.DiscountAmount,
                PaidAmount = entity.PaidAmount,
                PaymentMethod = entity.PaymentMethod,
                LiveCourseReserv = entity.LiveCourseReserv,
                RecordedCourseReserv = entity.RecordedCourseReserv,
                ExamSimulationReserv = entity.ExamSimulationReserv,
                TransactionId = entity.TransactionId,
                PaymentDate = entity.PaymentDate,
                EnrollmentStatusLookupId = entity.EnrollmentStatusLookupId,
                EnrollmentStatusName = entity.EnrollmentStatus?.LookupNameEn,
                EnrollmentDate = entity.EnrollmentDate,
                ExpiryDate = entity.ExpiryDate,
                CompletedDate = entity.CompletedDate,
                ProgressPercentage = entity.ProgressPercentage,
                CompletedLessons = entity.CompletedLessons,
                TotalLessons = entity.TotalLessons,
                IsCertificateIssued = entity.IsCertificateIssued,
                CertificateIssuedDate = entity.CertificateIssuedDate,
                CertificateNumber = entity.CertificateNumber
            };
        }
    }
}