using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseLiveSessionStudentRepository : Repository<CourseLiveSessionStudent>, ICourseLiveSessionStudentRepository
    {
        public CourseLiveSessionStudentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseLiveSessionStudent>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(clss => clss.LiveSession)
                .Include(clss => clss.Student)
                .Where(clss => !clss.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseLiveSessionStudent>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseLiveSessionStudent>> GetBySessionIdAsync(Guid sessionId)
        {
            return await _dbSet
                .Include(clss => clss.LiveSession)
                .Include(clss => clss.Student)
                .Where(clss => clss.CourseOid == sessionId && !clss.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<CourseLiveSessionStudent>> GetByStudentIdAsync(Guid studentId)
        {
            return await _dbSet
                .Include(clss => clss.LiveSession)
                .Include(clss => clss.Student)
                .Where(clss => clss.StudentOid == studentId && !clss.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid sessionId, Guid studentId)
        {
            return await _dbSet
                .AnyAsync(clss => clss.CourseOid == sessionId && clss.StudentOid == studentId && !clss.IsDeleted && clss.Active);
        }
    }
}