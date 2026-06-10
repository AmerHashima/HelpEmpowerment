using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ILinkService
    {
        Task<PagedResponse<LinkDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<LinkDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<LinkDto>> CreateAsync(CreateLinkDto dto);
        Task<ApiResponse<LinkDto>> UpdateAsync(UpdateLinkDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}