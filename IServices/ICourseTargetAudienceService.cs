using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseTargetAudienceService
    {
        Task<PagedResponse<CourseTargetAudienceDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseTargetAudienceDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseTargetAudienceDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseTargetAudienceDto>> CreateAsync(CreateCourseTargetAudienceDto dto);
        Task<ApiResponse<CourseTargetAudienceDto>> UpdateAsync(UpdateCourseTargetAudienceDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}