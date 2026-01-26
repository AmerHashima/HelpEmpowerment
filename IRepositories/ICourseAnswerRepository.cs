using StandardArticture.Common;
using StandardArticture.Models;

namespace StandardArticture.IRepositories
{
    public interface ICourseAnswerRepository : IRepository<CourseAnswer>
    {
        Task<PagedResult<CourseAnswer>> GetPagedAsync(DataRequest request);
        Task<IEnumerable<CourseAnswer>> GetByQuestionIdAsync(Guid questionId);
        Task<bool> DeleteByQuestionIdAsync(Guid questionId);
    }
}