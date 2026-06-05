using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseServiceDetailService
    {
        Task<PagedResponse<CourseServiceDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseServiceDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseServiceDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseServiceDto>> CreateAsync(CreateCourseServiceDto dto);
        Task<ApiResponse<CourseServiceDto>> UpdateAsync(UpdateCourseServiceDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
