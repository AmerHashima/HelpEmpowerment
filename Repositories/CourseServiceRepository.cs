using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Extensions;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseServiceRepository : Repository<CourseService>, ICourseServiceRepository
    {
        public CourseServiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseService>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Where(cs => !cs.IsDeleted)
                .Include(cs => cs.Course)
                .Include(cs => cs.ServiceLookup)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseService>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseService>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Where(cs => cs.CourseId == courseId && !cs.IsDeleted)
                .Include(cs => cs.ServiceLookup)
                .OrderBy(cs => cs.CreatedAt)
                .ToListAsync();
        }

        public async Task<CourseService?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Where(cs => cs.Oid == id && !cs.IsDeleted)
                .Include(cs => cs.Course)
                .Include(cs => cs.ServiceLookup)
                .FirstOrDefaultAsync();
        }
    }
}
