using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ============================================
        // USER AUTHENTICATION
        // ============================================

        /// <summary>
        /// Login for Users (Admin, Staff, etc.)
        /// </summary>
        [HttpPost("user/login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> LoginUser([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginUserAsync(dto);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        /// <summary>
        /// Register new User
        /// </summary>
        [HttpPost("user/register")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RegisterUser([FromBody] RegisterUserDto dto)
        {
            var response = await _authService.RegisterUserAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // ============================================
        // STUDENT AUTHENTICATION
        // ============================================

        /// <summary>
        /// Login for Students
        /// </summary>
        [HttpPost("student/login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> LoginStudent([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginStudentAsync(dto);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        /// <summary>
        /// Register new Student
        /// </summary>
        [HttpPost("student/register")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RegisterStudent([FromBody] RegisterStudentDto dto)
        {
            var response = await _authService.RegisterStudentAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // ============================================
        // PASSWORD MANAGEMENT
        // ============================================

        /// <summary>
        /// Request password reset (sends email with reset token)
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var response = await _authService.ForgotPasswordAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Reset password using reset token
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var response = await _authService.ResetPasswordAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Change password (requires authentication)
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var response = await _authService.ChangePasswordAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // ============================================
        // TOKEN MANAGEMENT
        // ============================================

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<TokenResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        /// <summary>
        /// Logout and revoke refresh token
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userType = User.FindFirst("UserType")?.Value ?? "Student";

            if (Guid.TryParse(userId, out var id))
            {
                var response = await _authService.RevokeTokenAsync(id, userType);
                return Ok(response);
            }

            return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid user"));
        }

        /// <summary>
        /// Validate token and get user info
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> GetCurrentUser()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _authService.ValidateTokenAsync(token);
            return response.Success ? Ok(response) : Unauthorized(response);
        }
    }
}