using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class AuthService : IAuthService
    {
        private const int MaxDevicesPerUser = 2;
        private const int MaxDevicesPerStudent = 2;

        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IStudentDeviceRepository _studentDeviceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            IUserDeviceRepository userDeviceRepository,
            IStudentDeviceRepository studentDeviceRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _userDeviceRepository = userDeviceRepository;
            _studentDeviceRepository = studentDeviceRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _logger = logger;
        }

        #region User Authentication

        public async Task<ApiResponse<LoginResponseDto>> LoginUserAsync(LoginDto dto)
        {
            try
            {
                var users = await _userRepository.FindAsync(
                    u => u.Username == dto.Username && !u.IsDeleted);
                var user = users.FirstOrDefault();

                if (user == null)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");

                if (!user.IsActive)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("User account is deactivated");

                if (!VerifyPassword(dto.Password, user.PasswordHash))
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");

                // Device tracking & max-device enforcement
                if (!string.IsNullOrWhiteSpace(dto.DeviceId))
                {
                    var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

                    var existingDevice = await _userDeviceRepository.GetByUserAndDeviceIdAsync(user.Oid, dto.DeviceId);
                    if (existingDevice != null)
                    {
                        // Known device — refresh last login info
                        existingDevice.LastLoginDate = DateTime.UtcNow;
                        existingDevice.IpAddress = ipAddress;
                        existingDevice.IsActive = true;
                        existingDevice.UpdatedAt = DateTime.UtcNow;
                        await _userDeviceRepository.UpdateAsync(existingDevice);
                    }
                    else
                    {
                        // New device — enforce the limit
                        var activeDeviceCount = await _userDeviceRepository.GetActiveDeviceCountAsync(user.Oid);
                        if (activeDeviceCount >= MaxDevicesPerUser)
                            return ApiResponse<LoginResponseDto>.ErrorResponse(
                                $"Maximum number of allowed devices ({MaxDevicesPerUser}) reached. " +
                                "Please remove an existing device before logging in from a new one.");

                        var newDevice = new UserDevice
                        {
                            UserId = user.Oid,
                            DeviceId = dto.DeviceId,
                            IpAddress = ipAddress,
                            IsActive = true,
                            FirstLoginDate = DateTime.UtcNow,
                            LastLoginDate = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _userDeviceRepository.AddAsync(newDevice);
                    }
                }

                // Generate tokens
                var token = GenerateJwtToken(user.Oid, user.Username, "User", user.RoleLookup?.LookupNameEn);
                var refreshToken = GenerateRefreshToken();

                // Update user with refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiration());
                user.LastLoginAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                var response = new LoginResponseDto
                {
                    UserId = user.Oid,
                    Username = user.Username,
                    Email = user.Email,

                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiration()),
                    UserType = "User",
                    Roles = user.RoleLookup != null ? new List<string> { user.RoleLookup.LookupNameEn } : new List<string>()
                };

                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return ApiResponse<LoginResponseDto>.ErrorResponse($"Login failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> RegisterUserAsync(RegisterUserDto dto)
        {
            try
            {
                // Validate password match
                if (dto.Password != dto.ConfirmPassword)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Passwords do not match");

                // Validate password strength
                var passwordValidation = ValidatePasswordStrength(dto.Password);
                if (!passwordValidation.IsValid)
                    return ApiResponse<LoginResponseDto>.ErrorResponse(passwordValidation.Message);

                // Check if username exists
                var usernameExists = await _userRepository.ExistsAsync(
                    u => u.Username == dto.Username && !u.IsDeleted);
                if (usernameExists)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Username already exists");

                // Check if email exists (if provided)
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var emailExists = await _userRepository.ExistsAsync(
                        u => u.Email == dto.Email && !u.IsDeleted);
                    if (emailExists)
                        return ApiResponse<LoginResponseDto>.ErrorResponse("Email already registered");
                }

                // Create user
                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = HashPassword(dto.Password),
                    Email = dto.Email,
             
                    RoleLookupId = dto.RoleLookupId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.AddAsync(user);

                // Generate tokens
                var token = GenerateJwtToken(createdUser.Oid, createdUser.Username, "User", null);
                var refreshToken = GenerateRefreshToken();

                // Update with refresh token
                createdUser.RefreshToken = refreshToken;
                createdUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiration());
                await _userRepository.UpdateAsync(createdUser);

                var response = new LoginResponseDto
                {
                    UserId = createdUser.Oid,
                    Username = createdUser.Username,
                    Email = createdUser.Email,
               
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiration()),
                    UserType = "User"
                };

                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return ApiResponse<LoginResponseDto>.ErrorResponse($"Registration failed: {ex.Message}");
            }
        }

        #endregion

        #region Student Authentication

        public async Task<ApiResponse<LoginResponseDto>> LoginStudentAsync(LoginDto dto)
        {
            try
            {
                var students = await _studentRepository.FindAsync(
                    s => s.Username == dto.Username && !s.IsDeleted);
                var student = students.FirstOrDefault();

                if (student == null)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");

                if (!student.IsActive)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Student account is deactivated");

                if (!VerifyPassword(dto.Password, student.PasswordHash))
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");

                // Device tracking for student login
                string? deviceId = dto.DeviceId;
                if (string.IsNullOrWhiteSpace(deviceId))
                    return ApiResponse<LoginResponseDto>.ErrorResponse("DeviceId is required for login");

                // Check if this device is already registered
                var existingDevice = await _studentDeviceRepository.GetByStudentAndDeviceIdAsync(student.Oid, deviceId);

                if (existingDevice != null)
                {
                    // Update existing device's last login time
                    existingDevice.LastLoginDate = DateTime.UtcNow;
                    await _studentDeviceRepository.UpdateAsync(existingDevice);
                }
                else
                {
                    // Check if student has max devices reached
                    int activeDeviceCount = await _studentDeviceRepository.GetActiveDeviceCountAsync(student.Oid);
                    if (activeDeviceCount >= MaxDevicesPerStudent)
                    {
                        return ApiResponse<LoginResponseDto>.ErrorResponse(
                            $"Maximum number of devices ({MaxDevicesPerStudent}) reached. Please logout from another device.");
                    }

                    // Register new device
                    var newDevice = new StudentDevice
                    {
                        StudentId = student.Oid,
                        DeviceId = deviceId,
                        IsActive = true,
                        FirstLoginDate = DateTime.UtcNow,
                        LastLoginDate = DateTime.UtcNow
                    };
                    await _studentDeviceRepository.AddAsync(newDevice);
                }

                // Generate tokens
                var token = GenerateJwtToken(student.Oid, student.Username, "Student", null);
                var refreshToken = GenerateRefreshToken();

                // Update student with refresh token
                student.RefreshToken = refreshToken;
                student.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiration());
                student.LastLoginAt = DateTime.UtcNow;
                await _studentRepository.UpdateAsync(student);

                var response = new LoginResponseDto
                {
                    UserId = student.Oid,
                    Username = student.Username,
                    Email = student.Email,
                    NameEn = student.NameEn,
                    NameAr = student.NameAr,
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiration()),
                    UserType = "Student"
                };

                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during student login");
                return ApiResponse<LoginResponseDto>.ErrorResponse($"Login failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> RegisterStudentAsync(RegisterStudentDto dto)
        {
            try
            {
                // Validate password match
                if (dto.Password != dto.ConfirmPassword)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Passwords do not match");

                // Validate password strength
                var passwordValidation = ValidatePasswordStrength(dto.Password);
                if (!passwordValidation.IsValid)
                    return ApiResponse<LoginResponseDto>.ErrorResponse(passwordValidation.Message);

                // Check if username exists
                var usernameExists = await _studentRepository.ExistsAsync(
                    s => s.Username == dto.Username && !s.IsDeleted);
                if (usernameExists)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Username already exists");

                // Check if email exists (if provided)
                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var emailExists = await _studentRepository.ExistsAsync(
                        s => s.Email == dto.Email && !s.IsDeleted);
                    if (emailExists)
                        return ApiResponse<LoginResponseDto>.ErrorResponse("Email already registered");
                }

                // Create student
                var student = new Student
                {
                    Username = dto.Username,
                    PasswordHash = HashPassword(dto.Password),
                    Email = dto.Email,
                    NameEn = dto.NameEn,
                    NameAr = dto.NameAr,
                    Mobile = dto.Mobile,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdStudent = await _studentRepository.AddAsync(student);

                // Generate tokens
                var token = GenerateJwtToken(createdStudent.Oid, createdStudent.Username, "Student", null);
                var refreshToken = GenerateRefreshToken();

                // Update with refresh token
                createdStudent.RefreshToken = refreshToken;
                createdStudent.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiration());
                await _studentRepository.UpdateAsync(createdStudent);

                var response = new LoginResponseDto
                {
                    UserId = createdStudent.Oid,
                    Username = createdStudent.Username,
                    Email = createdStudent.Email,
                    NameEn = createdStudent.NameEn,
                    NameAr = createdStudent.NameAr,
                    Token = token,
                    RefreshToken = refreshToken,
                    TokenExpires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiration()),
                    UserType = "Student"
                };

                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during student registration");
                return ApiResponse<LoginResponseDto>.ErrorResponse($"Registration failed: {ex.Message}");
            }
        }

        #endregion

        #region Password Management

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                var resetToken = GeneratePasswordResetToken();
                var tokenExpiry = DateTime.UtcNow.AddHours(24);

                if (dto.UserType.Equals("User", StringComparison.OrdinalIgnoreCase))
                {
                    var users = await _userRepository.FindAsync(
                        u => u.Email == dto.Email && !u.IsDeleted);
                    var user = users.FirstOrDefault();

                    if (user == null)
                        return ApiResponse<bool>.SuccessResponse(true, "If the email exists, a reset link has been sent");

                    user.PasswordResetToken = resetToken;
                    user.PasswordResetTokenExpiry = tokenExpiry;
                    await _userRepository.UpdateAsync(user);
                }
                else
                {
                    var students = await _studentRepository.FindAsync(
                        s => s.Email == dto.Email && !s.IsDeleted);
                    var student = students.FirstOrDefault();

                    if (student == null)
                        return ApiResponse<bool>.SuccessResponse(true, "If the email exists, a reset link has been sent");

                    student.PasswordResetToken = resetToken;
                    student.PasswordResetTokenExpiry = tokenExpiry;
                    await _studentRepository.UpdateAsync(student);
                }

                // TODO: Send email with reset token
                _logger.LogInformation($"Password reset token generated for {dto.Email}: {resetToken}");

                return ApiResponse<bool>.SuccessResponse(true, "If the email exists, a reset link has been sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password");
                return ApiResponse<bool>.ErrorResponse($"Failed to process request: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return ApiResponse<bool>.ErrorResponse("Passwords do not match");

                var passwordValidation = ValidatePasswordStrength(dto.NewPassword);
                if (!passwordValidation.IsValid)
                    return ApiResponse<bool>.ErrorResponse(passwordValidation.Message);

                // Try User first
                var users = await _userRepository.FindAsync(
                    u => u.Email == dto.Email && 
                         u.PasswordResetToken == dto.Token && 
                         u.PasswordResetTokenExpiry > DateTime.UtcNow &&
                         !u.IsDeleted);
                var user = users.FirstOrDefault();

                if (user != null)
                {
                    user.PasswordHash = HashPassword(dto.NewPassword);
                    user.PasswordResetToken = null;
                    user.PasswordResetTokenExpiry = null;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
                }

                // Try Student
                var students = await _studentRepository.FindAsync(
                    s => s.Email == dto.Email && 
                         s.PasswordResetToken == dto.Token && 
                         s.PasswordResetTokenExpiry > DateTime.UtcNow &&
                         !s.IsDeleted);
                var student = students.FirstOrDefault();

                if (student != null)
                {
                    student.PasswordHash = HashPassword(dto.NewPassword);
                    student.PasswordResetToken = null;
                    student.PasswordResetTokenExpiry = null;
                    student.UpdatedAt = DateTime.UtcNow;
                    await _studentRepository.UpdateAsync(student);
                    return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
                }

                return ApiResponse<bool>.ErrorResponse("Invalid or expired reset token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return ApiResponse<bool>.ErrorResponse($"Failed to reset password: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return ApiResponse<bool>.ErrorResponse("Passwords do not match");

                var passwordValidation = ValidatePasswordStrength(dto.NewPassword);
                if (!passwordValidation.IsValid)
                    return ApiResponse<bool>.ErrorResponse(passwordValidation.Message);

                // Try User first
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user != null)
                {
                    if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                        return ApiResponse<bool>.ErrorResponse("Current password is incorrect");

                    user.PasswordHash = HashPassword(dto.NewPassword);
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
                }

                // Try Student
                var student = await _studentRepository.GetByIdAsync(dto.UserId);
                if (student != null)
                {
                    if (!VerifyPassword(dto.CurrentPassword, student.PasswordHash))
                        return ApiResponse<bool>.ErrorResponse("Current password is incorrect");

                    student.PasswordHash = HashPassword(dto.NewPassword);
                    student.UpdatedAt = DateTime.UtcNow;
                    await _studentRepository.UpdateAsync(student);
                    return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
                }

                return ApiResponse<bool>.ErrorResponse("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return ApiResponse<bool>.ErrorResponse($"Failed to change password: {ex.Message}");
            }
        }

        #endregion

        #region Token Management

        public async Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(dto.Token);
                if (principal == null)
                    return ApiResponse<TokenResponseDto>.ErrorResponse("Invalid token");

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userTypeClaim = principal.FindFirst("UserType")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return ApiResponse<TokenResponseDto>.ErrorResponse("Invalid token");

                if (userTypeClaim == "User")
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user == null || user.RefreshToken != dto.RefreshToken || 
                        user.RefreshTokenExpiry < DateTime.UtcNow)
                        return ApiResponse<TokenResponseDto>.ErrorResponse("Invalid refresh token");

                    var newToken = GenerateJwtToken(user.Oid, user.Username, "User", user.RoleLookup?.LookupNameEn);
                    var newRefreshToken = GenerateRefreshToken();

                    user.RefreshToken = newRefreshToken;
                    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiration());
                    await _userRepository.UpdateAsync(user);

                    return ApiResponse<TokenResponseDto>.SuccessResponse(new TokenResponseDto
                    {
                        Token = newToken,
                        RefreshToken = newRefreshToken,
                        TokenExpires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiration())
                    });
                }
                else
                {
                    var student = await _studentRepository.GetByIdAsync(userId);
                    if (student == null || student.RefreshToken != dto.RefreshToken || 
                        student.RefreshTokenExpiry < DateTime.UtcNow)
                        return ApiResponse<TokenResponseDto>.ErrorResponse("Invalid refresh token");

                    var newToken = GenerateJwtToken(student.Oid, student.Username, "Student", null);
                    var newRefreshToken = GenerateRefreshToken();

                    student.RefreshToken = newRefreshToken;
                    student.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiration());
                    await _studentRepository.UpdateAsync(student);

                    return ApiResponse<TokenResponseDto>.SuccessResponse(new TokenResponseDto
                    {
                        Token = newToken,
                        RefreshToken = newRefreshToken,
                        TokenExpires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiration())
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return ApiResponse<TokenResponseDto>.ErrorResponse($"Failed to refresh token: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RevokeTokenAsync(Guid userId, string userType)
        {
            try
            {
                if (userType.Equals("User", StringComparison.OrdinalIgnoreCase))
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user != null)
                    {
                        user.RefreshToken = null;
                        user.RefreshTokenExpiry = null;
                        await _userRepository.UpdateAsync(user);
                    }
                }
                else
                {
                    var student = await _studentRepository.GetByIdAsync(userId);
                    if (student != null)
                    {
                        student.RefreshToken = null;
                        student.RefreshTokenExpiry = null;
                        await _studentRepository.UpdateAsync(student);
                    }
                }

                return ApiResponse<bool>.SuccessResponse(true, "Token revoked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token revocation");
                return ApiResponse<bool>.ErrorResponse($"Failed to revoke token: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> ValidateTokenAsync(string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token);
                if (principal == null)
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid token");

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userTypeClaim = principal.FindFirst("UserType")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid token");

                if (userTypeClaim == "User")
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user == null || !user.IsActive)
                        return ApiResponse<LoginResponseDto>.ErrorResponse("User not found or inactive");

                    return ApiResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
                    {
                        UserId = user.Oid,
                        Username = user.Username,
                        Email = user.Email,
             
                        UserType = "User"
                    });
                }
                else
                {
                    var student = await _studentRepository.GetByIdAsync(userId);
                    if (student == null || !student.IsActive)
                        return ApiResponse<LoginResponseDto>.ErrorResponse("Student not found or inactive");

                    return ApiResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
                    {
                        UserId = student.Oid,
                        Username = student.Username,
                        Email = student.Email,
                        NameEn = student.NameEn,
                        NameAr = student.NameAr,
                        UserType = "Student"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token validation");
                return ApiResponse<LoginResponseDto>.ErrorResponse($"Token validation failed: {ex.Message}");
            }
        }

        #endregion

        #region OTP Password Reset

        public async Task<ApiResponse<bool>> SendOtpAsync(ForgotPasswordOtpDto dto)
        {
            try
            {
                var otp = GenerateOtp();
                var expiry = DateTime.UtcNow.AddMinutes(10);

                if (dto.UserType.Equals("User", StringComparison.OrdinalIgnoreCase))
                {
                    var users = await _userRepository.FindAsync(
                        u => u.Email == dto.Email && !u.IsDeleted);
                    var user = users.FirstOrDefault();

                    // Always return success to avoid email enumeration
                    if (user == null)
                        return ApiResponse<bool>.SuccessResponse(true, "If the email exists, an OTP has been sent");

                    user.OtpCode = HashPassword(otp);
                    user.OtpExpiry = expiry;
                    await _userRepository.UpdateAsync(user);
                }
                else
                {
                    var students = await _studentRepository.FindAsync(
                        s => s.Email == dto.Email && !s.IsDeleted);
                    var student = students.FirstOrDefault();

                    if (student == null)
                        return ApiResponse<bool>.SuccessResponse(true, "If the email exists, an OTP has been sent");

                    student.OtpCode = HashPassword(otp);
                    student.OtpExpiry = expiry;
                    await _studentRepository.UpdateAsync(student);
                }

                // TODO: Send OTP via email/SMS
                _logger.LogInformation("OTP for {Email} ({UserType}): {Otp}", dto.Email, dto.UserType, otp);

                return ApiResponse<bool>.SuccessResponse(true, "OTP sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP");
                return ApiResponse<bool>.ErrorResponse($"Failed to send OTP: {ex.Message}");
            }
        }

        public async Task<ApiResponse<VerifyOtpResponseDto>> VerifyOtpAsync(VerifyOtpDto dto)
        {
            try
            {
                if (dto.UserType.Equals("User", StringComparison.OrdinalIgnoreCase))
                {
                    var users = await _userRepository.FindAsync(
                        u => u.Email == dto.Email && !u.IsDeleted);
                    var user = users.FirstOrDefault();

                    if (user == null || user.OtpCode == null || user.OtpExpiry == null)
                        return ApiResponse<VerifyOtpResponseDto>.ErrorResponse("Invalid or expired OTP");

                    if (user.OtpExpiry < DateTime.UtcNow)
                        return ApiResponse<VerifyOtpResponseDto>.ErrorResponse("OTP has expired");

                    if (!VerifyPassword(dto.OtpCode, user.OtpCode))
                        return ApiResponse<VerifyOtpResponseDto>.ErrorResponse("Invalid OTP");

                    // OTP verified — issue a short-lived reset token, clear OTP
                    var resetToken = GeneratePasswordResetToken();
                    user.PasswordResetToken = resetToken;
                    user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
                    user.OtpCode = null;
                    user.OtpExpiry = null;
                    await _userRepository.UpdateAsync(user);

                    return ApiResponse<VerifyOtpResponseDto>.SuccessResponse(new VerifyOtpResponseDto
                    {
                        ResetToken = resetToken,
                        Email = dto.Email,
                        UserType = dto.UserType
                    }, "OTP verified successfully");
                }
                else
                {
                    var students = await _studentRepository.FindAsync(
                        s => s.Email == dto.Email && !s.IsDeleted);
                    var student = students.FirstOrDefault();

                    if (student == null || student.OtpCode == null || student.OtpExpiry == null)
                        return ApiResponse<VerifyOtpResponseDto>.ErrorResponse("Invalid or expired OTP");

                    if (student.OtpExpiry < DateTime.UtcNow)
                        return ApiResponse<VerifyOtpResponseDto>.ErrorResponse("OTP has expired");

                    if (!VerifyPassword(dto.OtpCode, student.OtpCode))
                        return ApiResponse<VerifyOtpResponseDto>.ErrorResponse("Invalid OTP");

                    var resetToken = GeneratePasswordResetToken();
                    student.PasswordResetToken = resetToken;
                    student.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
                    student.OtpCode = null;
                    student.OtpExpiry = null;
                    await _studentRepository.UpdateAsync(student);

                    return ApiResponse<VerifyOtpResponseDto>.SuccessResponse(new VerifyOtpResponseDto
                    {
                        ResetToken = resetToken,
                        Email = dto.Email,
                        UserType = dto.UserType
                    }, "OTP verified successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP");
                return ApiResponse<VerifyOtpResponseDto>.ErrorResponse($"Failed to verify OTP: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordWithOtpAsync(ResetPasswordWithOtpDto dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return ApiResponse<bool>.ErrorResponse("Passwords do not match");

                var passwordValidation = ValidatePasswordStrength(dto.NewPassword);
                if (!passwordValidation.IsValid)
                    return ApiResponse<bool>.ErrorResponse(passwordValidation.Message);

                // Try User
                var users = await _userRepository.FindAsync(
                    u => u.Email == dto.Email &&
                         u.PasswordResetToken == dto.ResetToken &&
                         u.PasswordResetTokenExpiry > DateTime.UtcNow &&
                         !u.IsDeleted);
                var user = users.FirstOrDefault();

                if (user != null)
                {
                    user.PasswordHash = HashPassword(dto.NewPassword);
                    user.PasswordResetToken = null;
                    user.PasswordResetTokenExpiry = null;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
                }

                // Try Student
                var students = await _studentRepository.FindAsync(
                    s => s.Email == dto.Email &&
                         s.PasswordResetToken == dto.ResetToken &&
                         s.PasswordResetTokenExpiry > DateTime.UtcNow &&
                         !s.IsDeleted);
                var student = students.FirstOrDefault();

                if (student != null)
                {
                    student.PasswordHash = HashPassword(dto.NewPassword);
                    student.PasswordResetToken = null;
                    student.PasswordResetTokenExpiry = null;
                    student.UpdatedAt = DateTime.UtcNow;
                    await _studentRepository.UpdateAsync(student);
                    return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
                }

                return ApiResponse<bool>.ErrorResponse("Invalid or expired reset token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password with OTP");
                return ApiResponse<bool>.ErrorResponse($"Failed to reset password: {ex.Message}");
            }
        }

        #endregion

        #region Private Helper Methods

        private string GenerateJwtToken(Guid userId, string username, string userType, string? role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, username),
                new("UserType", userType),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpiration()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static string GeneratePasswordResetToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber).Replace("+", "").Replace("/", "").Replace("=", "");
        }

        private static string GenerateOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var value = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
            return value.ToString("D6");
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private int GetAccessTokenExpiration()
        {
            return int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "60");
        }

        private int GetRefreshTokenExpiration()
        {
            return int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!)),
                ValidateLifetime = false, // Allow expired tokens
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                        StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!)),
                ValidateLifetime = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private static (bool IsValid, string Message) ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return (false, "Password is required");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long");

            if (!password.Any(char.IsUpper))
                return (false, "Password must contain at least one uppercase letter");

            if (!password.Any(char.IsLower))
                return (false, "Password must contain at least one lowercase letter");

            if (!password.Any(char.IsDigit))
                return (false, "Password must contain at least one number");

            return (true, string.Empty);
        }

        #endregion
    }
}