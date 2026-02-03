using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseFeatureService
    {
        Task<PagedResponse<CourseFeatureDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseFeatureDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseFeatureDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseFeatureDto>> CreateAsync(CreateCourseFeatureDto dto);
        Task<ApiResponse<CourseFeatureDto>> UpdateAsync(UpdateCourseFeatureDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}