using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Extensions;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class LiveWebinarRepository : Repository<LiveWebinar>, ILiveWebinarRepository
    {
        public LiveWebinarRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<LiveWebinar>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<LiveWebinar>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }
    }
}
