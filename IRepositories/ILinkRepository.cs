using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ILinkRepository : IRepository<Link>
    {
        Task<PagedResult<Link>> GetPagedAsync(DataRequest request);
    }
}