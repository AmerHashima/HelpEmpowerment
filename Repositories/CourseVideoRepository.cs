using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseVideoRepository : Repository<CourseVideo>, ICourseVideoRepository
    {
        public CourseVideoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseVideo>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(cv => cv.Course)
                .Include(cv => cv.VideoTypeLookup)
                .Where(cv => !cv.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseVideo>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseVideo>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(cv => cv.Course)
                .Include(cv => cv.VideoTypeLookup)
                .Where(cv => cv.CourseOid == courseId && !cv.IsDeleted)
                .OrderBy(cv => cv.OrderNo)
                .ToListAsync();
        }

        public async Task<CourseVideo?> GetWithAttachmentsAsync(Guid id)
        {
            return await _dbSet
                .Include(cv => cv.Course)
                .Include(cv => cv.VideoTypeLookup)
                .Include(cv => cv.Attachments.Where(a => !a.IsDeleted))
                    .ThenInclude(a => a.FileTypeLookup)
                .FirstOrDefaultAsync(cv => cv.Oid == id && !cv.IsDeleted);
        }
    }
}