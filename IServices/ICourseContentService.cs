using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseContentService
    {
        Task<PagedResponse<CourseContentDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseContentDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseContentDto>>> GetByOutlineIdAsync(Guid outlineId);
        Task<ApiResponse<CourseContentDto>> CreateAsync(CreateCourseContentDto dto);
        Task<ApiResponse<CourseContentDto>> UpdateAsync(UpdateCourseContentDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}