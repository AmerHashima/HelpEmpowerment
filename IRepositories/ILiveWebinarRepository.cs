using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ILiveWebinarRepository : IRepository<LiveWebinar>
    {
        Task<PagedResult<LiveWebinar>> GetPagedAsync(DataRequest request);
    }
}
