using StandardArticture.Common;
using StandardArticture.Models;

namespace StandardArticture.IRepositories
{
    public interface ICoursesMasterExamRepository : IRepository<CoursesMasterExam>
    {
        Task<PagedResult<CoursesMasterExam>> GetPagedAsync(DataRequest request);
        Task<IEnumerable<CoursesMasterExam>> GetByCourseIdAsync(Guid courseId);
        Task<CoursesMasterExam?> GetWithQuestionsAsync(Guid id);
    }
}