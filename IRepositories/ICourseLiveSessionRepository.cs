using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseLiveSessionRepository : IRepository<CourseLiveSession>
    {
        Task<PagedResult<CourseLiveSession>> GetPagedAsync(DataRequest request);
        Task<List<CourseLiveSession>> GetByCourseIdAsync(Guid courseId);
        Task<CourseLiveSession?> GetWithStudentsAsync(Guid id);
        Task<List<CourseLiveSession>> GetUpcomingSessionsAsync(Guid courseId);
    }
}