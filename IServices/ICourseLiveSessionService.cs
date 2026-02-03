using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseLiveSessionService
    {
        Task<PagedResponse<CourseLiveSessionDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseLiveSessionDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseLiveSessionDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseLiveSessionDto>> GetWithStudentsAsync(Guid id);
        Task<ApiResponse<List<CourseLiveSessionDto>>> GetUpcomingSessionsAsync(Guid courseId);
        Task<ApiResponse<CourseLiveSessionDto>> CreateAsync(CreateCourseLiveSessionDto dto);
        Task<ApiResponse<CourseLiveSessionDto>> UpdateAsync(UpdateCourseLiveSessionDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}