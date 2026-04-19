using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ILiveWebinarService
    {
        Task<PagedResponse<LiveWebinarDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<LiveWebinarDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<LiveWebinarDto>> CreateAsync(CreateLiveWebinarDto dto);
        Task<ApiResponse<LiveWebinarDto>> UpdateAsync(UpdateLiveWebinarDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
