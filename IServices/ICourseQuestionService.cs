using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using Microsoft.AspNetCore.Http;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseQuestionService
    {
        Task<PagedResponse<CourseQuestionDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseQuestionDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseQuestionDto>>> GetByExamIdAsync(Guid examId);
        Task<ApiResponse<CourseQuestionDto>> GetWithAnswersAsync(Guid id);
        Task<ApiResponse<List<CourseQuestionDto>>> GetWithAnswersByExamIdAsync(Guid examId);
        Task<ApiResponse<CourseQuestionDto>> CreateAsync(CreateCourseQuestionDto dto);
        Task<ApiResponse<CourseQuestionDto>> UpdateAsync(UpdateCourseQuestionDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<CourseQuestionDto>> UploadImageAsync(Guid id, IFormFile image);
        Task<ApiResponse<string>> GetImagePathAsync(Guid id);
    }
}