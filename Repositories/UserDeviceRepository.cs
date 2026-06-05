using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class UserDeviceRepository : Repository<UserDevice>, IUserDeviceRepository
    {
        public UserDeviceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserDevice>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .OrderByDescending(d => d.LastLoginDate)
                .ToListAsync();
        }

        public async Task<UserDevice?> GetByUserAndDeviceIdAsync(Guid userId, string deviceId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId && !d.IsDeleted);
        }

        public async Task<int> GetActiveDeviceCountAsync(Guid userId)
        {
            return await _dbSet
                .CountAsync(d => d.UserId == userId && d.IsActive && !d.IsDeleted);
        }
    }
}
