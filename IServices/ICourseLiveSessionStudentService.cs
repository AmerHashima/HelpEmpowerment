using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseLiveSessionStudentService
    {
        Task<PagedResponse<CourseLiveSessionStudentDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseLiveSessionStudentDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseLiveSessionStudentDto>>> GetBySessionIdAsync(Guid sessionId);
        Task<ApiResponse<List<CourseLiveSessionStudentDto>>> GetByStudentIdAsync(Guid studentId);
        Task<ApiResponse<CourseLiveSessionStudentDto>> EnrollStudentAsync(CreateCourseLiveSessionStudentDto dto);
        Task<ApiResponse<bool>> UnenrollStudentAsync(Guid id);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}