using StandardArticture.Common;
using StandardArticture.Models;

namespace StandardArticture.IRepositories
{
    public interface ICourseQuestionRepository : IRepository<CourseQuestion>
    {
        Task<PagedResult<CourseQuestion>> GetPagedAsync(DataRequest request);
        Task<IEnumerable<CourseQuestion>> GetByExamIdAsync(Guid examId);
        Task<CourseQuestion?> GetWithAnswersAsync(Guid id);
        Task<IEnumerable<CourseQuestion>> GetWithAnswersByExamIdAsync(Guid examId);
    }
}