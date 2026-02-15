using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class StudentCourseRepository : Repository<StudentCourse>, IStudentCourseRepository
    {
        public StudentCourseRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PagedResult<StudentCourse>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Include(sc => sc.PaymentStatus)
                .Include(sc => sc.EnrollmentStatus)
                .Where(sc => !sc.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<StudentCourse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<StudentCourse>> GetByStudentIdAsync(Guid studentId)
        {
            return await _dbSet
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Include(sc => sc.PaymentStatus)
                .Include(sc => sc.EnrollmentStatus)
                .Where(sc => sc.StudentId == studentId && !sc.IsDeleted)
                .OrderByDescending(sc => sc.EnrollmentDate)
                .ToListAsync();
        }

        public async Task<List<StudentCourse>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(sc => sc.Student)
                .Include(sc => sc.PaymentStatus)
                .Include(sc => sc.EnrollmentStatus)
                .Where(sc => sc.CourseId == courseId && !sc.IsDeleted)
                .OrderByDescending(sc => sc.EnrollmentDate)
                .ToListAsync();
        }

        public async Task<StudentCourse?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
        {
            return await _dbSet
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Include(sc => sc.PaymentStatus)
                .Include(sc => sc.EnrollmentStatus)
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId && !sc.IsDeleted);
        }

        public async Task<StudentCourse?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Include(sc => sc.PaymentStatus)
                .Include(sc => sc.EnrollmentStatus)
                .FirstOrDefaultAsync(sc => sc.Oid == id && !sc.IsDeleted);
        }

        public async Task<List<StudentCourse>> GetByPaymentStatusAsync(Guid paymentStatusLookupId)
        {
            return await _dbSet
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Where(sc => sc.PaymentStatusLookupId == paymentStatusLookupId && !sc.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<StudentCourse>> GetByEnrollmentStatusAsync(Guid enrollmentStatusLookupId)
        {
            return await _dbSet
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Where(sc => sc.EnrollmentStatusLookupId == enrollmentStatusLookupId && !sc.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId)
        {
            return await _dbSet.AnyAsync(sc => 
                sc.StudentId == studentId && 
                sc.CourseId == courseId && 
                !sc.IsDeleted);
        }
    }
}