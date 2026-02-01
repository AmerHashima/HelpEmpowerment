using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICoursesMasterExamRepository : IRepository<CoursesMasterExam>
    {
        Task<PagedResult<CoursesMasterExam>> GetPagedAsync(DataRequest request);
        Task<IEnumerable<CoursesMasterExam>> GetByCourseIdAsync(Guid courseId);
        Task<CoursesMasterExam?> GetWithQuestionsAsync(Guid id);
    }
}