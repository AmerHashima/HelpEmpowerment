namespace HelpEmpowermentApi.DTOs
{
    // ============================================
    // LOGIN DTOs
    // ============================================
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Optional device tracking field (DeviceId is required for Student login)
        public string? DeviceId { get; set; }
    }

    public class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpires { get; set; }
        public string UserType { get; set; } = string.Empty; // "User" or "Student"
        public List<string>? Roles { get; set; }
    }

    // ============================================
    // REGISTER DTOs
    // ============================================
    public class RegisterUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? Mobile { get; set; }
        public Guid? RoleLookupId { get; set; }
    }

    public class RegisterStudentDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? Mobile { get; set; }
    }

    // ============================================
    // PASSWORD RESET DTOs
    // ============================================
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = "Student"; // "User" or "Student"
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }


    // ============================================
    // TOKEN DTOs
    // ============================================
    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpires { get; set; }
    }

    public class StudentLogoutDto
    {
        public string DeviceId { get; set; } = string.Empty;
    }

    // ============================================
    // OTP DTOs
    // ============================================

    public class ForgotPasswordOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = "Student"; // "User" or "Student"
    }

    public class VerifyOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string UserType { get; set; } = "Student"; // "User" or "Student"
    }

    public class VerifyOtpResponseDto
    {
        public string ResetToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }

    public class ResetPasswordWithOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
