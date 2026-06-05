using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class StudentDeviceRepository : Repository<StudentDevice>, IStudentDeviceRepository
    {
        public StudentDeviceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<StudentDevice?> GetByStudentAndDeviceIdAsync(Guid studentId, string deviceId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(sd => sd.StudentId == studentId && sd.DeviceId == deviceId && !sd.IsDeleted);
        }

        public async Task<int> GetActiveDeviceCountAsync(Guid studentId)
        {
            return await _dbSet
                .Where(sd => sd.StudentId == studentId && sd.IsActive && !sd.IsDeleted)
                .CountAsync();
        }

        public async Task<List<StudentDevice>> GetByStudentIdAsync(Guid studentId)
        {
            return await _dbSet
                .Where(sd => sd.StudentId == studentId && !sd.IsDeleted)
                .OrderByDescending(sd => sd.LastLoginDate)
                .ToListAsync();
        }
    }
}
