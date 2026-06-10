using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class RoleLinkService : IRoleLinkService
    {
        private readonly IRoleLinkRepository _repository;

        public RoleLinkService(IRoleLinkRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<RoleLinkDto>> GetPagedAsync(DataRequest request)
        {
            var result = await _repository.GetPagedAsync(request);

            return new PagedResponse<RoleLinkDto>
            {
                Success = true,
                Data = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<ApiResponse<RoleLinkDto>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return ApiResponse<RoleLinkDto>.ErrorResponse("RoleLink not found");

            return ApiResponse<RoleLinkDto>.SuccessResponse(MapToDto(entity));
        }

        public async Task<ApiResponse<RoleLinkDto>> CreateAsync(CreateRoleLinkDto dto)
        {
            var entity = new RoleLink
            {
                    RoleId = dto.RoleId,
                    LinkId = dto.LinkId,
                    CanRead = dto.CanRead,
                    CanWrite = dto.CanWrite,
                    CanEdit = dto.CanEdit,
                    CanDelete = dto.CanDelete,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy
            };

            var created = await _repository.AddAsync(entity);
            return ApiResponse<RoleLinkDto>.SuccessResponse(MapToDto(created), "RoleLink created successfully");
        }

        public async Task<ApiResponse<RoleLinkDto>> UpdateAsync(UpdateRoleLinkDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Oid);
            if (entity == null)
                return ApiResponse<RoleLinkDto>.ErrorResponse("RoleLink not found");

                entity.RoleId = dto.RoleId;
                entity.LinkId = dto.LinkId;
                entity.CanRead = dto.CanRead;
                entity.CanWrite = dto.CanWrite;
                entity.CanEdit = dto.CanEdit;
                entity.CanDelete = dto.CanDelete;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = dto.UpdatedBy;

            var updated = await _repository.UpdateAsync(entity);
            return ApiResponse<RoleLinkDto>.SuccessResponse(MapToDto(updated), "RoleLink updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            var result = await _repository.SoftDeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.ErrorResponse("RoleLink not found");

            return ApiResponse<bool>.SuccessResponse(true, "RoleLink deleted successfully");
        }

        private static RoleLinkDto MapToDto(RoleLink entity)
        {
            return new RoleLinkDto
            {
                Oid = entity.Oid,
                RoleId = entity.RoleId,
                LinkId = entity.LinkId,
                CanRead = entity.CanRead,
                CanWrite = entity.CanWrite,
                CanEdit = entity.CanEdit,
                CanDelete = entity.CanDelete,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}