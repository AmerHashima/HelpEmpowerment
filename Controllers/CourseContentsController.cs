using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseContentsController : ControllerBase
    {
        private readonly ICourseContentService _courseContentService;

        public CourseContentsController(ICourseContentService courseContentService)
        {
            _courseContentService = courseContentService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseContentDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _courseContentService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseContentDto>>> GetById(Guid id)
        {
            var response = await _courseContentService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("outline/{outlineId}")]
        public async Task<ActionResult<ApiResponse<List<CourseContentDto>>>> GetByOutlineId(Guid outlineId)
        {
            var response = await _courseContentService.GetByOutlineIdAsync(outlineId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseContentDto>>> Create([FromBody] CreateCourseContentDto dto)
        {
            var response = await _courseContentService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseContentDto>>> Update([FromBody] UpdateCourseContentDto dto)
        {
            var response = await _courseContentService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _courseContentService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}