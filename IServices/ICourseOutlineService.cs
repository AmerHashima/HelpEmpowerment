using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseOutlineService
    {
        Task<PagedResponse<CourseOutlineDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseOutlineDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseOutlineDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseOutlineDto>> GetWithContentsAsync(Guid id);
        Task<ApiResponse<CourseOutlineDto>> CreateAsync(CreateCourseOutlineDto dto);
        Task<ApiResponse<CourseOutlineDto>> UpdateAsync(UpdateCourseOutlineDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}