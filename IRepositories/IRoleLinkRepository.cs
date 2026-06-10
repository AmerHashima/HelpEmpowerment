using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IRoleLinkRepository : IRepository<RoleLink>
    {
        Task<PagedResult<RoleLink>> GetPagedAsync(DataRequest request);
    }
}