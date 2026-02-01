using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Extensions;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class AppLookupHeaderRepository : Repository<AppLookupHeader>, IAppLookupHeaderRepository
    {
        public AppLookupHeaderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<AppLookupHeader>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<AppLookupHeader>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<AppLookupHeader?> GetByCodeAsync(string lookupCode)
        {
            return await _dbSet
                .Include(h => h.LookupDetails.Where(d => !d.IsDeleted && d.IsActive))
                .FirstOrDefaultAsync(h => h.LookupCode == lookupCode && !h.IsDeleted);
        }

        public async Task<AppLookupHeader?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(h => h.LookupDetails.Where(d => !d.IsDeleted))
                .FirstOrDefaultAsync(h => h.Oid == id && !h.IsDeleted);
        }

        public async Task<bool> IsLookupCodeUniqueAsync(string lookupCode, Guid? excludeId = null)
        {
            var query = _dbSet.Where(h => h.LookupCode == lookupCode && !h.IsDeleted);
            
            if (excludeId.HasValue)
                query = query.Where(h => h.Oid != excludeId.Value);

            return !await query.AnyAsync();
        }
    }

    public class AppLookupDetailRepository : Repository<AppLookupDetail>, IAppLookupDetailRepository
    {
        public AppLookupDetailRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<AppLookupDetail>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(d => d.LookupHeader)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<AppLookupDetail>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<IEnumerable<AppLookupDetail>> GetByHeaderIdAsync(Guid headerId)
        {
            return await _dbSet
                .Where(d => d.LookupHeaderId == headerId && !d.IsDeleted && d.IsActive)
                .OrderBy(d => d.OrderNo)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppLookupDetail>> GetByHeaderCodeAsync(string headerCode)
        {
            return await _dbSet
                .Include(d => d.LookupHeader)
                .Where(d => d.LookupHeader.LookupCode == headerCode && !d.IsDeleted && d.IsActive)
                .OrderBy(d => d.OrderNo)
                .ToListAsync();
        }
    }
}