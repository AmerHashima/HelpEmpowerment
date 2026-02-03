using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseFeaturesController : ControllerBase
    {
        private readonly ICourseFeatureService _courseFeatureService;

        public CourseFeaturesController(ICourseFeatureService courseFeatureService)
        {
            _courseFeatureService = courseFeatureService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseFeatureDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _courseFeatureService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseFeatureDto>>> GetById(Guid id)
        {
            var response = await _courseFeatureService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CourseFeatureDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _courseFeatureService.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseFeatureDto>>> Create([FromBody] CreateCourseFeatureDto dto)
        {
            var response = await _courseFeatureService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseFeatureDto>>> Update([FromBody] UpdateCourseFeatureDto dto)
        {
            var response = await _courseFeatureService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _courseFeatureService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}