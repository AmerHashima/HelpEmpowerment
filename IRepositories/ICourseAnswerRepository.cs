using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseAnswerRepository : IRepository<CourseAnswer>
    {
        Task<PagedResult<CourseAnswer>> GetPagedAsync(DataRequest request);
        Task<List<CourseAnswer>> GetByQuestionIdAsync(Guid questionId);
        Task<CourseAnswer?> GetCorrectAnswerByQuestionIdAsync(Guid questionId);
    }
}