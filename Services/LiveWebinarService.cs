using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class LiveWebinarService : ILiveWebinarService
    {
        private readonly ILiveWebinarRepository _repository;

        public LiveWebinarService(ILiveWebinarRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<LiveWebinarDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _repository.GetPagedAsync(request);
                return new PagedResponse<LiveWebinarDto>
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
                return new PagedResponse<LiveWebinarDto> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<LiveWebinarDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null || entity.IsDeleted)
                    return ApiResponse<LiveWebinarDto>.ErrorResponse("Live webinar not found");

                return ApiResponse<LiveWebinarDto>.SuccessResponse(MapToDto(entity));
            }
            catch (Exception ex)
            {
                return ApiResponse<LiveWebinarDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LiveWebinarDto>> CreateAsync(CreateLiveWebinarDto dto)
        {
            try
            {
                var entity = new LiveWebinar
                {
                    WebinarName = dto.WebinarName,
                    WebinarFormat = dto.WebinarFormat,
                    WebinarDate = dto.WebinarDate,
                    WebinarStartTime = dto.WebinarStartTime,
                    WebinarEndTime = dto.WebinarEndTime,
                    TimeZone = dto.TimeZone,
                    WhatsAppLink = dto.WhatsAppLink,
                    Notes = dto.Notes,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(entity);
                return ApiResponse<LiveWebinarDto>.SuccessResponse(MapToDto(created), "Live webinar created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<LiveWebinarDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LiveWebinarDto>> UpdateAsync(UpdateLiveWebinarDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(dto.Oid);
                if (entity == null || entity.IsDeleted)
                    return ApiResponse<LiveWebinarDto>.ErrorResponse("Live webinar not found");

                entity.WebinarName = dto.WebinarName;
                entity.WebinarFormat = dto.WebinarFormat;
                entity.WebinarDate = dto.WebinarDate;
                entity.WebinarStartTime = dto.WebinarStartTime;
                entity.WebinarEndTime = dto.WebinarEndTime;
                entity.TimeZone = dto.TimeZone;
                entity.WhatsAppLink = dto.WhatsAppLink;
                entity.Notes = dto.Notes;
                entity.IsActive = dto.IsActive;
                entity.UpdatedBy = dto.UpdatedBy;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                return ApiResponse<LiveWebinarDto>.SuccessResponse(MapToDto(entity), "Live webinar updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<LiveWebinarDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Live webinar not found");

                return ApiResponse<bool>.SuccessResponse(true, "Live webinar deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static LiveWebinarDto MapToDto(LiveWebinar entity) => new()
        {
            Oid = entity.Oid,
            WebinarName = entity.WebinarName,
            WebinarFormat = entity.WebinarFormat,
            WebinarDate = entity.WebinarDate,
            WebinarStartTime = entity.WebinarStartTime,
            WebinarEndTime = entity.WebinarEndTime,
            TimeZone = entity.TimeZone,
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
