using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IRoleLinkService
    {
        Task<PagedResponse<RoleLinkDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<RoleLinkDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<RoleLinkDto>> CreateAsync(CreateRoleLinkDto dto);
        Task<ApiResponse<RoleLinkDto>> UpdateAsync(UpdateRoleLinkDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}