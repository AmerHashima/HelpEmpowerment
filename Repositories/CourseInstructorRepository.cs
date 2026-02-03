using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseInstructorRepository : Repository<CourseInstructor>, ICourseInstructorRepository
    {
        public CourseInstructorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseInstructor>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(ci => ci.Course)
                .Where(ci => !ci.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseInstructor>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseInstructor>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(ci => ci.Course)
                .Where(ci => ci.CourseOid == courseId && !ci.IsDeleted)
                .ToListAsync();
        }
    }
}