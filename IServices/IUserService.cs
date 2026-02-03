using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IUserService
    {
        Task<PagedResponse<UserDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<UserDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<UserDto>> GetByUsernameAsync(string username);
        Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto dto);
        Task<ApiResponse<UserDto>> UpdateAsync(UpdateUserDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto);
        Task<ApiResponse<UserDto>> AuthenticateAsync(string username, string password);
    }
}