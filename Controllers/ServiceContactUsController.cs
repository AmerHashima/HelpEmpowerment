using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceContactUsController : ControllerBase
    {
        private readonly IServiceContactUsService _service;

        public ServiceContactUsController(IServiceContactUsService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        [Authorize]
        public async Task<ActionResult<PagedResponse<ServiceContactUsDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _service.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ServiceContactUsDto>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("ticket/{ticketNumber}")]
        public async Task<ActionResult<ApiResponse<ServiceContactUsDto>>> GetByTicketNumber(string ticketNumber)
        {
            var response = await _service.GetByTicketNumberAsync(ticketNumber);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<ApiResponse<List<ServiceContactUsDto>>>> GetByStudentId(Guid studentId)
        {
            var response = await _service.GetByStudentIdAsync(studentId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("unread")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<ServiceContactUsDto>>>> GetUnread()
        {
            var response = await _service.GetUnreadAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("unread/count")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
        {
            var response = await _service.GetUnreadCountAsync();
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ServiceContactUsDto>>> Create([FromBody] CreateContactUsDto dto)
        {
            var response = await _service.CreateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{id}/respond")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ServiceContactUsDto>>> Respond(Guid id, [FromBody] RespondContactUsDto dto)
        {
            dto.Oid = id;
            var response = await _service.RespondAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(Guid id, [FromBody] MarkAsReadRequest request)
        {
            var response = await _service.MarkAsReadAsync(id, request.ReadBy);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            var response = await _service.UpdateStatusAsync(id, request.StatusLookupId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _service.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }

    public class MarkAsReadRequest
    {
        public Guid ReadBy { get; set; }
    }

    public class UpdateStatusRequest
    {
        public Guid StatusLookupId { get; set; }
    }
}