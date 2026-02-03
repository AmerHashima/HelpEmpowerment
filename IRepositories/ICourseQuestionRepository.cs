using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseQuestionRepository : IRepository<CourseQuestion>
    {
        Task<PagedResult<CourseQuestion>> GetPagedAsync(DataRequest request);
        Task<CourseQuestion?> GetWithAnswersAsync(Guid id);
        Task<List<CourseQuestion>> GetByMasterExamIdAsync(Guid masterExamId);
        Task<List<CourseQuestion>> GetByExamIdAsync(Guid examId);
        Task<List<CourseQuestion>> GetWithAnswersByExamIdAsync(Guid examId);
    }
}