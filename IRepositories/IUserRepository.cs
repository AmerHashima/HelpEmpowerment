using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<PagedResult<User>> GetPagedAsync(DataRequest request);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeId = null);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
        Task<User?> AuthenticateAsync(string username, string passwordHash);
    }
}