using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseContentRepository : Repository<CourseContent>, ICourseContentRepository
    {
        public CourseContentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseContent>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(cc => cc.CourseOutline)
                .Include(cc => cc.ContentTypeLookup)
                .Where(cc => !cc.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseContent>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseContent>> GetByOutlineIdAsync(Guid outlineId)
        {
            return await _dbSet
                .Include(cc => cc.CourseOutline)
                .Include(cc => cc.ContentTypeLookup)
                .Where(cc => cc.CourseOutlineOid == outlineId && !cc.IsDeleted)
                .OrderBy(cc => cc.OrderNo)
                .ToListAsync();
        }
    }
}