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
            var normalizedRequest = NormalizeRequest(request);

            var query = _dbSet
                .Where(r => !r.IsDeleted)
                .Include(r => r.CourseService)
                    .ThenInclude(cs => cs.Course)
                .Include(r => r.CourseService)
                    .ThenInclude(cs => cs.ServiceLookup)
                .Include(r => r.StudentCourse)
                .AsQueryable();

            query = query.ApplyFilters(normalizedRequest.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(normalizedRequest.Sort);
            query = query.ApplyPagination(normalizedRequest.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<StudentCourseReservation>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = normalizedRequest.Pagination.PageNumber,
                PageSize = normalizedRequest.Pagination.PageSize
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
                .Include(r => r.StudentCourse)
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

        public async Task<List<string>> GetExistingServiceValuesAsync(
            Guid studentId,
            Guid courseId,
            IEnumerable<string> serviceValues)
        {
            var values = serviceValues.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (values.Count == 0) return [];

            return await _dbSet.AsNoTracking()
                .Where(reservation => !reservation.IsDeleted
                    && !reservation.StudentCourse.IsDeleted
                    && reservation.StudentCourse.StudentId == studentId
                    && reservation.StudentCourse.CourseId == courseId
                    && values.Contains(reservation.CourseService.ServiceLookup.LookupValue))
                .Select(reservation => reservation.CourseService.ServiceLookup.LookupValue)
                .Distinct()
                .ToListAsync();
        }

        private static DataRequest NormalizeRequest(DataRequest request)
        {
            var normalized = new DataRequest
            {
                Pagination = request.Pagination,
                Filters = request.Filters?.Select(filter => new FilterRequest
                {
                    PropertyName = NormalizePropertyPath(filter.PropertyName),
                    Operation = filter.Operation,
                    Value = filter.Value
                }).ToList() ?? [],
                Sort = request.Sort?.Select(sort => new SortRequest
                {
                    SortBy = NormalizePropertyPath(sort.SortBy),
                    SortDirection = sort.SortDirection
                }).ToList() ?? []
            };

            return normalized;
        }

        private static string NormalizePropertyPath(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return string.Empty;

            return propertyName.Trim() switch
            {
                "StudentId" => "StudentCourse.StudentId",
                "CourseId" => "StudentCourse.CourseId",
                _ => propertyName
            };
        }
    }
}
