using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<UserDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _userService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
        {
            var response = await _userService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetByUsername(string username)
        {
            var response = await _userService.GetByUsernameAsync(username);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto dto)
        {
            var response = await _userService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<UserDto>>> Update([FromBody] UpdateUserDto dto)
        {
            var response = await _userService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _userService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var response = await _userService.ChangePasswordAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Authenticate([FromBody] LoginDto dto)
        {
            var response = await _userService.AuthenticateAsync(dto.Username, dto.Password);
            return response.Success ? Ok(response) : Unauthorized(response);
        }
    }

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}