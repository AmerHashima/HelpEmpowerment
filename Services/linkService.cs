using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class LinkService : ILinkService
    {
        private readonly ILinkRepository _repository;

        public LinkService(ILinkRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<LinkDto>> GetPagedAsync(DataRequest request)
        {
            var result = await _repository.GetPagedAsync(request);

            return new PagedResponse<LinkDto>
            {
                Success = true,
                Data = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<ApiResponse<LinkDto>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return ApiResponse<LinkDto>.ErrorResponse("Link not found");

            return ApiResponse<LinkDto>.SuccessResponse(MapToDto(entity));
        }

        public async Task<ApiResponse<LinkDto>> CreateAsync(CreateLinkDto dto)
        {
            var entity = new Link
            {
                    NameAr = dto.NameAr,
                    NameEn = dto.NameEn,
                    Icon = dto.Icon,
                    Type = dto.Type,
                    Active = dto.Active,
                    Path = dto.Path,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy
            };

            var created = await _repository.AddAsync(entity);
            return ApiResponse<LinkDto>.SuccessResponse(MapToDto(created), "Link created successfully");
        }

        public async Task<ApiResponse<LinkDto>> UpdateAsync(UpdateLinkDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Oid);
            if (entity == null)
                return ApiResponse<LinkDto>.ErrorResponse("Link not found");

                entity.NameAr = dto.NameAr;
                entity.NameEn = dto.NameEn;
                entity.Icon = dto.Icon;
                entity.Type = dto.Type;
                entity.Active = dto.Active;
                entity.Path = dto.Path;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = dto.UpdatedBy;

            var updated = await _repository.UpdateAsync(entity);
            return ApiResponse<LinkDto>.SuccessResponse(MapToDto(updated), "Link updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            var result = await _repository.SoftDeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.ErrorResponse("Link not found");

            return ApiResponse<bool>.SuccessResponse(true, "Link deleted successfully");
        }

        private static LinkDto MapToDto(Link entity)
        {
            return new LinkDto
            {
                Oid = entity.Oid,
                NameAr = entity.NameAr,
                NameEn = entity.NameEn,
                Icon = entity.Icon,
                Type = entity.Type,
                Active = entity.Active,
                Path = entity.Path,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}