using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IAppLookupService
    {
        Task<PagedResponse<AppLookupHeaderDto>> GetHeadersPagedAsync(DataRequest request);
        Task<ApiResponse<AppLookupHeaderDto>> GetHeaderByIdAsync(Guid id);
        Task<ApiResponse<AppLookupHeaderDto>> GetHeaderByCodeAsync(string lookupCode);
        Task<ApiResponse<AppLookupHeaderDto>> CreateHeaderAsync(CreateAppLookupHeaderDto dto);
        Task<ApiResponse<AppLookupHeaderDto>> UpdateHeaderAsync(UpdateAppLookupHeaderDto dto);
        Task<ApiResponse<bool>> DeleteHeaderAsync(Guid id);

        Task<PagedResponse<AppLookupDetailDto>> GetDetailsPagedAsync(DataRequest request);
        Task<ApiResponse<AppLookupDetailDto>> GetDetailByIdAsync(Guid id);
        Task<ApiResponse<List<AppLookupDetailDto>>> GetDetailsByHeaderIdAsync(Guid headerId);
        Task<ApiResponse<List<AppLookupDetailDto>>> GetDetailsByHeaderCodeAsync(string headerCode);
        Task<ApiResponse<AppLookupDetailDto>> CreateDetailAsync(CreateAppLookupDetailDto dto);
        Task<ApiResponse<AppLookupDetailDto>> UpdateDetailAsync(UpdateAppLookupDetailDto dto);
        Task<ApiResponse<bool>> DeleteDetailAsync(Guid id);
    }
}