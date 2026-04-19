using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ILiveCourseService
    {
        Task<PagedResponse<LiveCourseDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<LiveCourseDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<LiveCourseDto>> CreateAsync(CreateLiveCourseDto dto);
        Task<ApiResponse<LiveCourseDto>> UpdateAsync(UpdateLiveCourseDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
