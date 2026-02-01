using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IAppLookupHeaderRepository : IRepository<AppLookupHeader>
    {
        Task<PagedResult<AppLookupHeader>> GetPagedAsync(DataRequest request);
        Task<AppLookupHeader?> GetByCodeAsync(string lookupCode);
        Task<AppLookupHeader?> GetWithDetailsAsync(Guid id);
        Task<bool> IsLookupCodeUniqueAsync(string lookupCode, Guid? excludeId = null);
    }

    public interface IAppLookupDetailRepository : IRepository<AppLookupDetail>
    {
        Task<PagedResult<AppLookupDetail>> GetPagedAsync(DataRequest request);
        Task<IEnumerable<AppLookupDetail>> GetByHeaderIdAsync(Guid headerId);
        Task<IEnumerable<AppLookupDetail>> GetByHeaderCodeAsync(string headerCode);
    }
}