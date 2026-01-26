using StandardArticture.Common;
using StandardArticture.Models;

namespace StandardArticture.IRepositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<PagedResult<Course>> GetPagedAsync(DataRequest request);
        Task<Course?> GetByCodeAsync(string courseCode);
        Task<bool> IsCourseCodeUniqueAsync(string courseCode, Guid? excludeId = null);
    }
}