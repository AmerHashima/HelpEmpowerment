using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<RoleDto>> GetPagedAsync(DataRequest request)
        {
            var result = await _repository.GetPagedAsync(request);

            return new PagedResponse<RoleDto>
            {
                Success = true,
                Data = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<ApiResponse<RoleDto>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return ApiResponse<RoleDto>.ErrorResponse("Role not found");

            return ApiResponse<RoleDto>.SuccessResponse(MapToDto(entity));
        }

        public async Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto dto)
        {
            var entity = new Role
            {
                    Name = dto.Name,
                    Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = dto.CreatedBy
            };

            var created = await _repository.AddAsync(entity);
            return ApiResponse<RoleDto>.SuccessResponse(MapToDto(created), "Role created successfully");
        }

        public async Task<ApiResponse<RoleDto>> UpdateAsync(UpdateRoleDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Oid);
            if (entity == null)
                return ApiResponse<RoleDto>.ErrorResponse("Role not found");

                entity.Name = dto.Name;
                entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = dto.UpdatedBy;

            var updated = await _repository.UpdateAsync(entity);
            return ApiResponse<RoleDto>.SuccessResponse(MapToDto(updated), "Role updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            var result = await _repository.SoftDeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.ErrorResponse("Role not found");

            return ApiResponse<bool>.SuccessResponse(true, "Role deleted successfully");
        }

        private static RoleDto MapToDto(Role entity)
        {
            return new RoleDto
            {
                Oid = entity.Oid,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}