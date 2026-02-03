using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseOutlinesController : ControllerBase
    {
        private readonly ICourseOutlineService _courseOutlineService;

        public CourseOutlinesController(ICourseOutlineService courseOutlineService)
        {
            _courseOutlineService = courseOutlineService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseOutlineDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _courseOutlineService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseOutlineDto>>> GetById(Guid id)
        {
            var response = await _courseOutlineService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CourseOutlineDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _courseOutlineService.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("{id}/with-contents")]
        public async Task<ActionResult<ApiResponse<CourseOutlineDto>>> GetWithContents(Guid id)
        {
            var response = await _courseOutlineService.GetWithContentsAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseOutlineDto>>> Create([FromBody] CreateCourseOutlineDto dto)
        {
            var response = await _courseOutlineService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseOutlineDto>>> Update([FromBody] UpdateCourseOutlineDto dto)
        {
            var response = await _courseOutlineService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _courseOutlineService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}