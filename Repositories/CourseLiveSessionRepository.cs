using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseLiveSessionRepository : Repository<CourseLiveSession>, ICourseLiveSessionRepository
    {
        public CourseLiveSessionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseLiveSession>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(cls => cls.Course)
                .Where(cls => !cls.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseLiveSession>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseLiveSession>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(cls => cls.Course)
                .Where(cls => cls.CourseOid == courseId && !cls.IsDeleted)
                .OrderBy(cls => cls.Date)
                .ToListAsync();
        }

        public async Task<CourseLiveSession?> GetWithStudentsAsync(Guid id)
        {
            return await _dbSet
                .Include(cls => cls.Course)
                .Include(cls => cls.SessionStudents.Where(ss => !ss.IsDeleted))
                    .ThenInclude(ss => ss.Student)
                .FirstOrDefaultAsync(cls => cls.Oid == id && !cls.IsDeleted);
        }

        public async Task<List<CourseLiveSession>> GetUpcomingSessionsAsync(Guid courseId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(cls => cls.Course)
                .Where(cls => cls.CourseOid == courseId && cls.Date >= now && !cls.IsDeleted && cls.Active)
                .OrderBy(cls => cls.Date)
                .ToListAsync();
        }
    }
}