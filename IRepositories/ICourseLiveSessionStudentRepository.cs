using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseLiveSessionStudentRepository : IRepository<CourseLiveSessionStudent>
    {
        Task<PagedResult<CourseLiveSessionStudent>> GetPagedAsync(DataRequest request);
        Task<List<CourseLiveSessionStudent>> GetBySessionIdAsync(Guid sessionId);
        Task<List<CourseLiveSessionStudent>> GetByStudentIdAsync(Guid studentId);
        Task<bool> IsStudentEnrolledAsync(Guid sessionId, Guid studentId);
    }
}