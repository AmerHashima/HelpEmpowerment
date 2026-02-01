using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseQuestionRepository : IRepository<CourseQuestion>
    {
        Task<PagedResult<CourseQuestion>> GetPagedAsync(DataRequest request);
        Task<IEnumerable<CourseQuestion>> GetByExamIdAsync(Guid examId);
        Task<CourseQuestion?> GetWithAnswersAsync(Guid id);
        Task<IEnumerable<CourseQuestion>> GetWithAnswersByExamIdAsync(Guid examId);
    }
}