using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IStudentExamService
    {
        Task<PagedResponse<StudentExamDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<StudentExamDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<StudentExamDto>>> GetByStudentIdAsync(Guid studentId);
        Task<ApiResponse<StudentExamDto>> GetWithQuestionsAsync(Guid id);
        Task<ApiResponse<StudentExamDto>> StartExamAsync(CreateStudentExamDto dto);
        Task<ApiResponse<StudentExamDto>> SubmitExamAsync(SubmitStudentExamDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}