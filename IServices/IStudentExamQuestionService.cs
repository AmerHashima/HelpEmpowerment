using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IStudentExamQuestionService
    {
        Task<PagedResponse<StudentExamQuestionDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<StudentExamQuestionDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<StudentExamQuestionDto>>> GetByStudentExamIdAsync(Guid studentExamId);
        Task<ApiResponse<StudentExamQuestionDto>> CreateAsync(CreateStudentExamQuestionDto dto);
        Task<ApiResponse<StudentExamQuestionDto>> UpdateAsync(UpdateStudentExamQuestionDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}