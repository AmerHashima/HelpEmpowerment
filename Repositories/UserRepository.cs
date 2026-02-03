using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<User>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(u => u.RoleLookup)
                .Include(u => u.StatusLookup)
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<User>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .Include(u => u.RoleLookup)
                .Include(u => u.StatusLookup)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.RoleLookup)
                .Include(u => u.StatusLookup)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeId = null)
        {
            var query = _dbSet.Where(u => u.Username == username && !u.IsDeleted);
            
            if (excludeId.HasValue)
                query = query.Where(u => u.Oid != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true;

            var query = _dbSet.Where(u => u.Email == email && !u.IsDeleted);
            
            if (excludeId.HasValue)
                query = query.Where(u => u.Oid != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<User?> AuthenticateAsync(string username, string passwordHash)
        {
            return await _dbSet
                .Include(u => u.RoleLookup)
                .Include(u => u.StatusLookup)
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash && !u.IsDeleted && u.IsActive);
        }
    }
}