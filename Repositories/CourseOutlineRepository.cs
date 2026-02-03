using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseOutlineRepository : Repository<CourseOutline>, ICourseOutlineRepository
    {
        public CourseOutlineRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseOutline>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(co => co.Course)
                .Where(co => !co.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseOutline>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseOutline>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(co => co.Course)
                .Where(co => co.CourseOid == courseId && !co.IsDeleted)
                .OrderBy(co => co.OrderNo)
                .ToListAsync();
        }

        public async Task<CourseOutline?> GetWithContentsAsync(Guid id)
        {
            return await _dbSet
                .Include(co => co.Course)
                .Include(co => co.Contents.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.ContentTypeLookup)
                .FirstOrDefaultAsync(co => co.Oid == id && !co.IsDeleted);
        }
    }
}