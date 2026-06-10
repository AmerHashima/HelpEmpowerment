using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<PagedResult<Role>> GetPagedAsync(DataRequest request);
    }
}