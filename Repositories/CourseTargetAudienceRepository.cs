using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseTargetAudienceRepository : Repository<CourseTargetAudience>, ICourseTargetAudienceRepository
    {
        public CourseTargetAudienceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseTargetAudience>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(cta => cta.Course)
                .Where(cta => !cta.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseTargetAudience>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseTargetAudience>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(cta => cta.Course)
                .Where(cta => cta.CourseOid == courseId && !cta.IsDeleted)
                .OrderBy(cta => cta.OrderNo)
                .ToListAsync();
        }
    }
}