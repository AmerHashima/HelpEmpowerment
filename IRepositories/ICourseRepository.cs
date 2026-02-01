using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<PagedResult<Course>> GetPagedAsync(DataRequest request);
        Task<Course?> GetByCodeAsync(string courseCode);
        Task<bool> IsCourseCodeUniqueAsync(string courseCode, Guid? excludeId = null);
    }
}