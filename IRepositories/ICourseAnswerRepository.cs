using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseAnswerRepository : IRepository<CourseAnswer>
    {
        Task<PagedResult<CourseAnswer>> GetPagedAsync(DataRequest request);
        Task<IEnumerable<CourseAnswer>> GetByQuestionIdAsync(Guid questionId);
        Task<bool> DeleteByQuestionIdAsync(Guid questionId);
    }
}