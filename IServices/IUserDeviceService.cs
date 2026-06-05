using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IUserDeviceService
    {
        Task<ApiResponse<IEnumerable<UserDeviceDto>>> GetDevicesByUserIdAsync(Guid userId);
        Task<ApiResponse<bool>> RemoveDeviceAsync(RemoveUserDeviceDto dto);
        Task<ApiResponse<bool>> DeactivateAllDevicesAsync(Guid userId);
    }
}
