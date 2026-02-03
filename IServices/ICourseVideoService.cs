using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseVideoService
    {
        Task<PagedResponse<CourseVideoDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseVideoDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseVideoDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseVideoDto>> GetWithAttachmentsAsync(Guid id);
        Task<ApiResponse<CourseVideoDto>> CreateAsync(CreateCourseVideoDto dto);
        Task<ApiResponse<CourseVideoDto>> UpdateAsync(UpdateCourseVideoDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}