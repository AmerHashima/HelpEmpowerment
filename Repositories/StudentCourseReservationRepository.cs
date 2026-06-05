using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Extensions;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class StudentCourseReservationRepository : Repository<StudentCourseReservation>, IStudentCourseReservationRepository
    {
        public StudentCourseReservationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<StudentCourseReservation>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Where(r => !r.IsDeleted)
                .Include(r => r.CourseService)
                    .ThenInclude(cs => cs.Course)
                .Include(r => r.CourseService)
                    .ThenInclude(cs => cs.ServiceLookup)
                .Include(r => r.StudentCourse)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<StudentCourseReservation>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<StudentCourseReservation>> GetByStudentCourseIdAsync(Guid studentCourseId)
        {
            return await _dbSet
                .Where(r => r.StudentCourseId == studentCourseId && !r.IsDeleted)
                .Include(r => r.CourseService)
                    .ThenInclude(cs => cs.Course)
                .Include(r => r.CourseService)
                    .ThenInclude(cs => cs.ServiceLookup)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<StudentCourseReservation?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Where(r => r.Oid == id && !r.IsDeleted)
                .Include(r => r.CourseService)
                    .ThenInclude(cs => cs.Course)
                .Include(r => r.CourseService)
                    .ThenInclude(cs => cs.ServiceLookup)
                .Include(r => r.StudentCourse)
                .FirstOrDefaultAsync();
        }
    }
}
