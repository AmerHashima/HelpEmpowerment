using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IRoleService
    {
        Task<PagedResponse<RoleDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<RoleDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto dto);
        Task<ApiResponse<RoleDto>> UpdateAsync(UpdateRoleDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}