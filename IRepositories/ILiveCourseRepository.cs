using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ILiveCourseRepository : IRepository<LiveCourse>
    {
        Task<PagedResult<LiveCourse>> GetPagedAsync(DataRequest request);
    }
}
