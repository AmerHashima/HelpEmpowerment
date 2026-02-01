using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Extensions;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Course>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(c => c.CourseLevelLookup)
                .Include(c => c.CourseCategoryLookup)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<Course>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<Course?> GetByCodeAsync(string courseCode)
        {
            return await _dbSet
                .Include(c => c.CourseLevelLookup)
                .Include(c => c.CourseCategoryLookup)
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode && !c.IsDeleted);
        }

        public async Task<bool> IsCourseCodeUniqueAsync(string courseCode, Guid? excludeId = null)
        {
            var query = _dbSet.Where(c => c.CourseCode == courseCode && !c.IsDeleted);
            
            if (excludeId.HasValue)
                query = query.Where(c => c.Oid != excludeId.Value);

            return !await query.AnyAsync();
        }
    }
}