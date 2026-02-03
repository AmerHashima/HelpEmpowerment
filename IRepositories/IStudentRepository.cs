using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<PagedResult<Student>> GetPagedAsync(DataRequest request);
        Task<Student?> GetByUsernameAsync(string username);
        Task<Student?> GetByEmailAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeId = null);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
        Task<Student?> AuthenticateAsync(string username, string passwordHash);
    }
}