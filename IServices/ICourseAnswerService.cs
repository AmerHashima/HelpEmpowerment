using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseAnswerService
    {
        Task<PagedResponse<CourseAnswerDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseAnswerDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseAnswerDto>>> GetByQuestionIdAsync(Guid questionId);
        Task<ApiResponse<CourseAnswerDto>> GetCorrectAnswerByQuestionIdAsync(Guid questionId);
        Task<ApiResponse<CourseAnswerDto>> CreateAsync(CreateCourseAnswerDto dto);
        Task<ApiResponse<CourseAnswerDto>> UpdateAsync(UpdateCourseAnswerDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}