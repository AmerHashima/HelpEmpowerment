using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IUserDeviceRepository : IRepository<UserDevice>
    {
        Task<IEnumerable<UserDevice>> GetByUserIdAsync(Guid userId);
        Task<UserDevice?> GetByUserAndDeviceIdAsync(Guid userId, string deviceId);
        Task<int> GetActiveDeviceCountAsync(Guid userId);
    }
}
