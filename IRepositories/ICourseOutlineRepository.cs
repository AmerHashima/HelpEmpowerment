using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseOutlineRepository : IRepository<CourseOutline>
    {
        Task<PagedResult<CourseOutline>> GetPagedAsync(DataRequest request);
        Task<List<CourseOutline>> GetByCourseIdAsync(Guid courseId);
        Task<CourseOutline?> GetWithContentsAsync(Guid id);
    }
}