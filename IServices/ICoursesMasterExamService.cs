using StandardArticture.Common;
using StandardArticture.DTOs;

namespace StandardArticture.IServices
{
    public interface ICoursesMasterExamService
    {
        Task<PagedResponse<CoursesMasterExamDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CoursesMasterExamDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CoursesMasterExamDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CoursesMasterExamDto>> GetWithQuestionsAsync(Guid id);
        Task<ApiResponse<CoursesMasterExamDto>> CreateAsync(CreateCoursesMasterExamDto dto);
        Task<ApiResponse<CoursesMasterExamDto>> UpdateAsync(UpdateCoursesMasterExamDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}