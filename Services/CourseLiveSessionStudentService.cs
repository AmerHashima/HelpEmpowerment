using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseLiveSessionStudentService : ICourseLiveSessionStudentService
    {
        private readonly ICourseLiveSessionStudentRepository _sessionStudentRepository;
        private readonly ICourseLiveSessionRepository _liveSessionRepository;
        private readonly IStudentRepository _studentRepository;

        public CourseLiveSessionStudentService(
            ICourseLiveSessionStudentRepository sessionStudentRepository,
            ICourseLiveSessionRepository liveSessionRepository,
            IStudentRepository studentRepository)
        {
            _sessionStudentRepository = sessionStudentRepository;
            _liveSessionRepository = liveSessionRepository;
            _studentRepository = studentRepository;
        }

        public async Task<PagedResponse<CourseLiveSessionStudentDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _sessionStudentRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseLiveSessionStudentDto>
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
                return new PagedResponse<CourseLiveSessionStudentDto>
                {
                    Success = false,
                    Message = $"Error retrieving session enrollments: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseLiveSessionStudentDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var sessionStudent = await _sessionStudentRepository.GetByIdAsync(id);
                if (sessionStudent == null)
                    return ApiResponse<CourseLiveSessionStudentDto>.ErrorResponse("Session enrollment not found");

                return ApiResponse<CourseLiveSessionStudentDto>.SuccessResponse(MapToDto(sessionStudent));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseLiveSessionStudentDto>.ErrorResponse($"Error retrieving session enrollment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseLiveSessionStudentDto>>> GetBySessionIdAsync(Guid sessionId)
        {
            try
            {
                var sessionStudents = await _sessionStudentRepository.GetBySessionIdAsync(sessionId);
                var dtos = sessionStudents.Select(MapToDto).ToList();

                return ApiResponse<List<CourseLiveSessionStudentDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseLiveSessionStudentDto>>.ErrorResponse($"Error retrieving session enrollments: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseLiveSessionStudentDto>>> GetByStudentIdAsync(Guid studentId)
        {
            try
            {
                var sessionStudents = await _sessionStudentRepository.GetByStudentIdAsync(studentId);
                var dtos = sessionStudents.Select(MapToDto).ToList();

                return ApiResponse<List<CourseLiveSessionStudentDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseLiveSessionStudentDto>>.ErrorResponse($"Error retrieving student enrollments: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseLiveSessionStudentDto>> EnrollStudentAsync(CreateCourseLiveSessionStudentDto dto)
        {
            try
            {
                // Validate Live Session exists
                var session = await _liveSessionRepository.GetByIdAsync(dto.CourseOid);
                if (session == null)
                    return ApiResponse<CourseLiveSessionStudentDto>.ErrorResponse("Invalid Live Session. Please select a valid session.");

                // Validate Student exists
                var studentExists = await _studentRepository.ExistsAsync(s => s.Oid == dto.StudentOid && !s.IsDeleted);
                if (!studentExists)
                    return ApiResponse<CourseLiveSessionStudentDto>.ErrorResponse("Invalid Student. Please select a valid student.");

                // Check if student is already enrolled
                var isEnrolled = await _sessionStudentRepository.IsStudentEnrolledAsync(dto.CourseOid, dto.StudentOid);
                if (isEnrolled)
                    return ApiResponse<CourseLiveSessionStudentDto>.ErrorResponse("Student is already enrolled in this session.");

                // Check if session is full
                if (session.MaxNumberReservation.HasValue && 
                    session.NumberOfReservations >= session.MaxNumberReservation.Value)
                    return ApiResponse<CourseLiveSessionStudentDto>.ErrorResponse("Session is full. Maximum reservations reached.");

                var sessionStudent = new CourseLiveSessionStudent
                {
                    CourseOid = dto.CourseOid,
                    StudentOid = dto.StudentOid,
                    Active = dto.Active,
                    CreatedAt = DateTime.UtcNow
                };

                var createdEnrollment = await _sessionStudentRepository.AddAsync(sessionStudent);

                // Update session reservation count
                session.NumberOfReservations = (session.NumberOfReservations ?? 0) + 1;
                await _liveSessionRepository.UpdateAsync(session);

                return ApiResponse<CourseLiveSessionStudentDto>.SuccessResponse(MapToDto(createdEnrollment), "Student enrolled successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseLiveSessionStudentDto>.ErrorResponse($"Error enrolling student: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UnenrollStudentAsync(Guid id)
        {
            try
            {
                var sessionStudent = await _sessionStudentRepository.GetByIdAsync(id);
                if (sessionStudent == null)
                    return ApiResponse<bool>.ErrorResponse("Session enrollment not found");

                // Get the session to update reservation count
                var session = await _liveSessionRepository.GetByIdAsync(sessionStudent.CourseOid);
                
                // Soft delete the enrollment
                var result = await _sessionStudentRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Failed to unenroll student");

                // Update session reservation count
                if (session != null && session.NumberOfReservations > 0)
                {
                    session.NumberOfReservations = session.NumberOfReservations - 1;
                    await _liveSessionRepository.UpdateAsync(session);
                }

                return ApiResponse<bool>.SuccessResponse(true, "Student unenrolled successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error unenrolling student: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _sessionStudentRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Session enrollment not found");

                return ApiResponse<bool>.SuccessResponse(true, "Session enrollment deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting session enrollment: {ex.Message}");
            }
        }

        private CourseLiveSessionStudentDto MapToDto(CourseLiveSessionStudent sessionStudent)
        {
            return new CourseLiveSessionStudentDto
            {
                Oid = sessionStudent.Oid,
                CourseOid = sessionStudent.CourseOid,
                StudentOid = sessionStudent.StudentOid,
                StudentName = sessionStudent.Student?.NameEn ?? sessionStudent.Student?.Username,
                StudentEmail = sessionStudent.Student?.Email,
                Active = sessionStudent.Active,
                CreatedAt = sessionStudent.CreatedAt
            };
        }
    }
}