using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseServiceRepository : IRepository<CourseService>
    {
        Task<PagedResult<CourseService>> GetPagedAsync(DataRequest request);
        Task<List<CourseService>> GetByCourseIdAsync(Guid courseId);
        Task<CourseService?> GetWithDetailsAsync(Guid id);
    }
}
