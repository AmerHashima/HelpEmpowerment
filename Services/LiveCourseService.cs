using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class LiveCourseService : ILiveCourseService
    {
        private readonly ILiveCourseRepository _repository;

        public LiveCourseService(ILiveCourseRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<LiveCourseDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _repository.GetPagedAsync(request);
                return new PagedResponse<LiveCourseDto>
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
                return new PagedResponse<LiveCourseDto> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<LiveCourseDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null || entity.IsDeleted)
                    return ApiResponse<LiveCourseDto>.ErrorResponse("Live course not found");

                return ApiResponse<LiveCourseDto>.SuccessResponse(MapToDto(entity));
            }
            catch (Exception ex)
            {
                return ApiResponse<LiveCourseDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LiveCourseDto>> CreateAsync(CreateLiveCourseDto dto)
        {
            try
            {
                var entity = new LiveCourse
                {
                    CourseName = dto.CourseName,
                    CourseOid = dto.CourseOid,
                    CourseFormat = dto.CourseFormat,
                    StartDate = dto.StartDate,
                    StartTime = dto.StartTime,
                    TimeZone = dto.TimeZone,
                    NumberOfSessions = dto.NumberOfSessions,
                    TotalHours = dto.TotalHours,
                    ScheduleNotes = dto.ScheduleNotes,
                    WhatsAppLink = dto.WhatsAppLink,
                    Notes = dto.Notes,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(entity);
                return ApiResponse<LiveCourseDto>.SuccessResponse(MapToDto(created), "Live course created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<LiveCourseDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LiveCourseDto>> UpdateAsync(UpdateLiveCourseDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(dto.Oid);
                if (entity == null || entity.IsDeleted)
                    return ApiResponse<LiveCourseDto>.ErrorResponse("Live course not found");

                entity.CourseName = dto.CourseName;
                entity.CourseOid = dto.CourseOid;
                entity.CourseFormat = dto.CourseFormat;
                entity.StartDate = dto.StartDate;
                entity.StartTime = dto.StartTime;
                entity.TimeZone = dto.TimeZone;
                entity.NumberOfSessions = dto.NumberOfSessions;
                entity.TotalHours = dto.TotalHours;
                entity.ScheduleNotes = dto.ScheduleNotes;
                entity.WhatsAppLink = dto.WhatsAppLink;
                entity.Notes = dto.Notes;
                entity.IsActive = dto.IsActive;
                entity.UpdatedBy = dto.UpdatedBy;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                return ApiResponse<LiveCourseDto>.SuccessResponse(MapToDto(entity), "Live course updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<LiveCourseDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Live course not found");

                return ApiResponse<bool>.SuccessResponse(true, "Live course deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static LiveCourseDto MapToDto(LiveCourse entity) => new()
        {
            Oid = entity.Oid,
            CourseName = entity.CourseName,
            CourseOid = entity.CourseOid,
            CourseRefName = entity.Course?.CourseName,
            CourseFormat = entity.CourseFormat,
            StartDate = entity.StartDate,
            StartTime = entity.StartTime,
            TimeZone = entity.TimeZone,
            NumberOfSessions = entity.NumberOfSessions,
            TotalHours = entity.TotalHours,
            ScheduleNotes = entity.ScheduleNotes,
            WhatsAppLink = entity.WhatsAppLink,
            Notes = entity.Notes,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy
        };
    }
}
