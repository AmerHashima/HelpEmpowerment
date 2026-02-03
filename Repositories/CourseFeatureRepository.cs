using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseFeatureRepository : Repository<CourseFeature>, ICourseFeatureRepository
    {
        public CourseFeatureRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseFeature>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(cf => cf.Course)
                .Where(cf => !cf.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseFeature>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseFeature>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(cf => cf.Course)
                .Where(cf => cf.CourseOid == courseId && !cf.IsDeleted)
                .OrderBy(cf => cf.OrderNo)
                .ToListAsync();
        }
    }
}