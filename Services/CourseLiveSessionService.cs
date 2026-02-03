using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseLiveSessionService : ICourseLiveSessionService
    {
        private readonly ICourseLiveSessionRepository _liveSessionRepository;
        private readonly ICourseRepository _courseRepository;

        public CourseLiveSessionService(
            ICourseLiveSessionRepository liveSessionRepository,
            ICourseRepository courseRepository)
        {
            _liveSessionRepository = liveSessionRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResponse<CourseLiveSessionDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _liveSessionRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseLiveSessionDto>
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
                return new PagedResponse<CourseLiveSessionDto>
                {
                    Success = false,
                    Message = $"Error retrieving live sessions: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseLiveSessionDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var session = await _liveSessionRepository.GetByIdAsync(id);
                if (session == null)
                    return ApiResponse<CourseLiveSessionDto>.ErrorResponse("Live session not found");

                return ApiResponse<CourseLiveSessionDto>.SuccessResponse(MapToDto(session));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseLiveSessionDto>.ErrorResponse($"Error retrieving live session: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseLiveSessionDto>>> GetByCourseIdAsync(Guid courseId)
        {
            try
            {
                var sessions = await _liveSessionRepository.GetByCourseIdAsync(courseId);
                var dtos = sessions.Select(MapToDto).ToList();

                return ApiResponse<List<CourseLiveSessionDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseLiveSessionDto>>.ErrorResponse($"Error retrieving live sessions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseLiveSessionDto>> GetWithStudentsAsync(Guid id)
        {
            try
            {
                var session = await _liveSessionRepository.GetWithStudentsAsync(id);
                if (session == null)
                    return ApiResponse<CourseLiveSessionDto>.ErrorResponse("Live session not found");

                return ApiResponse<CourseLiveSessionDto>.SuccessResponse(MapToDtoWithStudents(session));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseLiveSessionDto>.ErrorResponse($"Error retrieving live session: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseLiveSessionDto>>> GetUpcomingSessionsAsync(Guid courseId)
        {
            try
            {
                var sessions = await _liveSessionRepository.GetUpcomingSessionsAsync(courseId);
                var dtos = sessions.Select(MapToDto).ToList();

                return ApiResponse<List<CourseLiveSessionDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseLiveSessionDto>>.ErrorResponse($"Error retrieving upcoming sessions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseLiveSessionDto>> CreateAsync(CreateCourseLiveSessionDto dto)
        {
            try
            {
                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseLiveSessionDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                var session = new CourseLiveSession
                {
                    CourseOid = dto.CourseOid,
                    TimeFrom = dto.TimeFrom,
                    TimeTo = dto.TimeTo,
                    Date = dto.Date,
                    MaxNumberReservation = dto.MaxNumberReservation,
                    NumberOfReservations = dto.NumberOfReservations,
                    Active = dto.Active,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdSession = await _liveSessionRepository.AddAsync(session);
                return ApiResponse<CourseLiveSessionDto>.SuccessResponse(MapToDto(createdSession), "Live session created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseLiveSessionDto>.ErrorResponse($"Error creating live session: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseLiveSessionDto>> UpdateAsync(UpdateCourseLiveSessionDto dto)
        {
            try
            {
                var session = await _liveSessionRepository.GetByIdAsync(dto.Oid);
                if (session == null)
                    return ApiResponse<CourseLiveSessionDto>.ErrorResponse("Live session not found");

                // Validate Course exists
                var courseExists = await _courseRepository.ExistsAsync(c => c.Oid == dto.CourseOid && !c.IsDeleted);
                if (!courseExists)
                    return ApiResponse<CourseLiveSessionDto>.ErrorResponse("Invalid Course. Please select a valid course.");

                session.CourseOid = dto.CourseOid;
                session.TimeFrom = dto.TimeFrom;
                session.TimeTo = dto.TimeTo;
                session.Date = dto.Date;
                session.MaxNumberReservation = dto.MaxNumberReservation;
                session.NumberOfReservations = dto.NumberOfReservations;
                session.Active = dto.Active;
                session.UpdatedBy = dto.UpdatedBy;
                session.UpdatedAt = DateTime.UtcNow;

                var updatedSession = await _liveSessionRepository.UpdateAsync(session);
                return ApiResponse<CourseLiveSessionDto>.SuccessResponse(MapToDto(updatedSession), "Live session updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseLiveSessionDto>.ErrorResponse($"Error updating live session: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _liveSessionRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Live session not found");

                return ApiResponse<bool>.SuccessResponse(true, "Live session deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting live session: {ex.Message}");
            }
        }

        private CourseLiveSessionDto MapToDto(CourseLiveSession session)
        {
            return new CourseLiveSessionDto
            {
                Oid = session.Oid,
                CourseOid = session.CourseOid,
                CourseName = session.Course?.CourseName,
                TimeFrom = session.TimeFrom,
                TimeTo = session.TimeTo,
                Date = session.Date,
                MaxNumberReservation = session.MaxNumberReservation,
                NumberOfReservations = session.NumberOfReservations,
                Active = session.Active,
                CreatedAt = session.CreatedAt,
                CreatedBy = session.CreatedBy,
                UpdatedAt = session.UpdatedAt,
                UpdatedBy = session.UpdatedBy
            };
        }

        private CourseLiveSessionDto MapToDtoWithStudents(CourseLiveSession session)
        {
            return new CourseLiveSessionDto
            {
                Oid = session.Oid,
                CourseOid = session.CourseOid,
                CourseName = session.Course?.CourseName,
                TimeFrom = session.TimeFrom,
                TimeTo = session.TimeTo,
                Date = session.Date,
                MaxNumberReservation = session.MaxNumberReservation,
                NumberOfReservations = session.NumberOfReservations,
                Active = session.Active,
                CreatedAt = session.CreatedAt,
                CreatedBy = session.CreatedBy,
                UpdatedAt = session.UpdatedAt,
                UpdatedBy = session.UpdatedBy,
                SessionStudents = session.SessionStudents.Select(ss => new CourseLiveSessionStudentDto
                {
                    Oid = ss.Oid,
                    CourseOid = ss.CourseOid,
                    StudentOid = ss.StudentOid,
                    StudentName = ss.Student?.NameEn ?? ss.Student?.Username,
                    StudentEmail = ss.Student?.Email,
                    Active = ss.Active,
                    CreatedAt = ss.CreatedAt
                }).ToList()
            };
        }
    }
}