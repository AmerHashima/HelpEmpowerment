using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseTargetAudiencesController : ControllerBase
    {
        private readonly ICourseTargetAudienceService _targetAudienceService;

        public CourseTargetAudiencesController(ICourseTargetAudienceService targetAudienceService)
        {
            _targetAudienceService = targetAudienceService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResponse<CourseTargetAudienceDto>>> Search([FromBody] DataRequest request)
        {
            var response = await _targetAudienceService.GetPagedAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseTargetAudienceDto>>> GetById(Guid id)
        {
            var response = await _targetAudienceService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<ApiResponse<List<CourseTargetAudienceDto>>>> GetByCourseId(Guid courseId)
        {
            var response = await _targetAudienceService.GetByCourseIdAsync(courseId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseTargetAudienceDto>>> Create([FromBody] CreateCourseTargetAudienceDto dto)
        {
            var response = await _targetAudienceService.CreateAsync(dto);
            return response.Success ? CreatedAtAction(nameof(GetById), new { id = response.Data?.Oid }, response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<CourseTargetAudienceDto>>> Update([FromBody] UpdateCourseTargetAudienceDto dto)
        {
            var response = await _targetAudienceService.UpdateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _targetAudienceService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}