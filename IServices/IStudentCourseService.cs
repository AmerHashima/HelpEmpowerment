using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IStudentCourseService
    {
        Task<PagedResponse<StudentCourseDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<StudentCourseDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<StudentCourseDto>>> GetByStudentIdAsync(Guid studentId);
        Task<ApiResponse<List<StudentCourseDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<StudentCourseDto>> EnrollStudentAsync(CreateStudentCourseDto dto);
        Task<ApiResponse<StudentCourseDto>> UpdateAsync(UpdateStudentCourseDto dto);
        Task<ApiResponse<StudentCourseDto>> UpdatePaymentStatusAsync(Guid id, Guid paymentStatusLookupId, string? transactionId);
        Task<ApiResponse<StudentCourseDto>> UpdateProgressAsync(Guid id, int completedLessons, int totalLessons);
        Task<ApiResponse<bool>> IsStudentEnrolledAsync(Guid studentId, Guid courseId);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}