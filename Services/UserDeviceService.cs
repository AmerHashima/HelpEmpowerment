using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Services
{
    public class UserDeviceService : IUserDeviceService
    {
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly ILogger<UserDeviceService> _logger;

        public UserDeviceService(IUserDeviceRepository userDeviceRepository, ILogger<UserDeviceService> logger)
        {
            _userDeviceRepository = userDeviceRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<UserDeviceDto>>> GetDevicesByUserIdAsync(Guid userId)
        {
            try
            {
                var devices = await _userDeviceRepository.GetByUserIdAsync(userId);
                var dtos = devices.Select(MapToDto);
                return ApiResponse<IEnumerable<UserDeviceDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving devices for user {UserId}", userId);
                return ApiResponse<IEnumerable<UserDeviceDto>>.ErrorResponse($"Error retrieving devices: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RemoveDeviceAsync(RemoveUserDeviceDto dto)
        {
            try
            {
                var device = await _userDeviceRepository.GetByIdAsync(dto.DeviceOid);

                if (device == null)
                    return ApiResponse<bool>.ErrorResponse("Device not found");

                if (device.UserId != dto.UserId)
                    return ApiResponse<bool>.ErrorResponse("Device does not belong to this user");

                device.IsActive = false;
                device.UpdatedAt = DateTime.UtcNow;
                await _userDeviceRepository.UpdateAsync(device);

                return ApiResponse<bool>.SuccessResponse(true, "Device removed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing device {DeviceOid}", dto.DeviceOid);
                return ApiResponse<bool>.ErrorResponse($"Error removing device: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateAllDevicesAsync(Guid userId)
        {
            try
            {
                var devices = await _userDeviceRepository.GetByUserIdAsync(userId);
                foreach (var device in devices.Where(d => d.IsActive))
                {
                    device.IsActive = false;
                    device.UpdatedAt = DateTime.UtcNow;
                    await _userDeviceRepository.UpdateAsync(device);
                }

                return ApiResponse<bool>.SuccessResponse(true, "All devices deactivated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating devices for user {UserId}", userId);
                return ApiResponse<bool>.ErrorResponse($"Error deactivating devices: {ex.Message}");
            }
        }

        private static UserDeviceDto MapToDto(Models.UserDevice device) => new()
        {
            Oid = device.Oid,
            UserId = device.UserId,
            DeviceId = device.DeviceId,
            IpAddress = device.IpAddress,
            IsActive = device.IsActive,
            FirstLoginDate = device.FirstLoginDate,
            LastLoginDate = device.LastLoginDate
        };
    }
}
