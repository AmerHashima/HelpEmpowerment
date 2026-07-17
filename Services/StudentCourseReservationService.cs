using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Repositories;

namespace HelpEmpowermentApi.Services
{
    public class StudentCourseReservationService : IStudentCourseReservationService
    {
        private readonly IStudentCourseReservationRepository _repository;
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly ICourseServiceRepository _courseServiceRepository;

        public StudentCourseReservationService(
            IStudentCourseReservationRepository repository,
            IStudentCourseRepository studentCourseRepository,
            ICourseServiceRepository courseServiceRepository)
        {
            _repository = repository;
            _studentCourseRepository = studentCourseRepository;
            _courseServiceRepository = courseServiceRepository;
        }

        public async Task<PagedResponse<StudentCourseReservationDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _repository.GetPagedAsync(request);
                return new PagedResponse<StudentCourseReservationDto>
                {
                    Success = true,
                    Data = pagedResult.Items.Select(MapToDto).ToList(),
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                return new PagedResponse<StudentCourseReservationDto> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<StudentCourseReservationDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetWithDetailsAsync(id);
                if (entity == null)
                    return ApiResponse<StudentCourseReservationDto>.ErrorResponse("Reservation not found");

                return ApiResponse<StudentCourseReservationDto>.SuccessResponse(MapToDto(entity));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentCourseReservationDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<StudentCourseReservationDto>>> GetByStudentCourseIdAsync(Guid studentCourseId)
        {
            try
            {
                var entities = await _repository.GetByStudentCourseIdAsync(studentCourseId);
                return ApiResponse<List<StudentCourseReservationDto>>.SuccessResponse(entities.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<StudentCourseReservationDto>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentCourseReservationDto>> CreateAsync(CreateStudentCourseReservationDto dto)
        {
            try
            {
                var enrollment = await _studentCourseRepository.GetByIdAsync(dto.StudentCourseId);
                if (enrollment == null)
                    return ApiResponse<StudentCourseReservationDto>.ErrorResponse("Student enrollment not found");
                var courseService = await _courseServiceRepository.GetByIdAsync(dto.CourseServiceId);
                if (courseService == null)
                    return ApiResponse<StudentCourseReservationDto>.ErrorResponse("Course service not found");
                var entity = new StudentCourseReservation
                {
                    StudentCourseId = dto.StudentCourseId,
                    CourseServiceId = dto.CourseServiceId,
                    ReservationDate = dto.ReservationDate,
                    ReservationExpiryDate = dto.ReservationDate?.AddDays(courseService?.ActiveTime ?? 0), // Example: set expiry date based on CourseService active time
                    ServicePrice = dto.ServicePrice,
                    IsReserved = dto.IsReserved,
                    Notes = dto.Notes,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(entity);
                var result = await _repository.GetWithDetailsAsync(created.Oid);
                return ApiResponse<StudentCourseReservationDto>.SuccessResponse(MapToDto(result!), "Reservation created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentCourseReservationDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentCourseReservationDto>> UpdateAsync(UpdateStudentCourseReservationDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(dto.Oid);
                if (entity == null)
                    return ApiResponse<StudentCourseReservationDto>.ErrorResponse("Reservation not found");

                entity.CourseServiceId = dto.CourseServiceId;
                entity.ReservationDate = dto.ReservationDate;
                entity.IsReserved = dto.IsReserved;
                entity.Notes = dto.Notes;
                entity.UpdatedBy = dto.UpdatedBy;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                var result = await _repository.GetWithDetailsAsync(dto.Oid);
                return ApiResponse<StudentCourseReservationDto>.SuccessResponse(MapToDto(result!), "Reservation updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentCourseReservationDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Reservation not found");

                return ApiResponse<bool>.SuccessResponse(true, "Reservation deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static StudentCourseReservationDto MapToDto(StudentCourseReservation entity) => new()
        {
            Oid = entity.Oid,
            StudentCourseId = entity.StudentCourseId,
            StudentId = entity.StudentCourse?.StudentId,
            CourseId = entity.StudentCourse?.CourseId,
            CourseServiceId = entity.CourseServiceId,
            CourseName = entity.CourseService?.Course?.CourseName,
            ServiceName = entity.CourseService?.ServiceLookup?.LookupNameEn,
            ServicePrice = entity.ServicePrice,
            ActiveTime = entity.CourseService?.ActiveTime,
            ReservationDate = entity.ReservationDate,
            ReservationExpiryDate = entity.ReservationExpiryDate,
            IsReserved = entity.IsReserved,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy
        };
    }
}
