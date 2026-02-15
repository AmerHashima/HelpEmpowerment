using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IAuthService
    {
        // User Authentication
        Task<ApiResponse<LoginResponseDto>> LoginUserAsync(LoginDto dto);
        Task<ApiResponse<LoginResponseDto>> RegisterUserAsync(RegisterUserDto dto);
        
        // Student Authentication
        Task<ApiResponse<LoginResponseDto>> LoginStudentAsync(LoginDto dto);
        Task<ApiResponse<LoginResponseDto>> RegisterStudentAsync(RegisterStudentDto dto);
        
        // Password Management
        Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto);
        
        // Token Management
        Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
        Task<ApiResponse<bool>> RevokeTokenAsync(Guid userId, string userType);
        
        // Validation
        Task<ApiResponse<LoginResponseDto>> ValidateTokenAsync(string token);
    }
}