using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<StudentDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _studentService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(Guid id)
        {
            var response = await _studentService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> GetByUsername(string username)
        {
            var response = await _studentService.GetByUsernameAsync(username);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudentDto>>> Create([FromBody] CreateStudentDto dto)
        {
            var response = await _studentService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<StudentDto>>> Update([FromBody] UpdateStudentDto dto)
        {
            var response = await _studentService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _studentService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> Authenticate([FromBody] StudentLoginDto dto)
        {
            var response = await _studentService.AuthenticateAsync(dto.Username, dto.Password);
            return response.Success ? Ok(response) : Unauthorized(response);
        }
    }

    public class StudentLoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}