using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Student>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Where(s => !s.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<Student>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<Student?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Username == username && !s.IsDeleted);
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeId = null)
        {
            var query = _dbSet.Where(s => s.Username == username && !s.IsDeleted);
            
            if (excludeId.HasValue)
                query = query.Where(s => s.Oid != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true;

            var query = _dbSet.Where(s => s.Email == email && !s.IsDeleted);
            
            if (excludeId.HasValue)
                query = query.Where(s => s.Oid != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<Student?> AuthenticateAsync(string username, string passwordHash)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Username == username && s.PasswordHash == passwordHash && !s.IsDeleted && s.IsActive);
        }
    }
}