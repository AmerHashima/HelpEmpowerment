using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IStudentDeviceRepository : IRepository<StudentDevice>
    {
        Task<StudentDevice?> GetByStudentAndDeviceIdAsync(Guid studentId, string deviceId);
        Task<int> GetActiveDeviceCountAsync(Guid studentId);
        Task<List<StudentDevice>> GetByStudentIdAsync(Guid studentId);
    }
}
