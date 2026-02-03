using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseVideoAttachmentRepository : Repository<CourseVideoAttachment>, ICourseVideoAttachmentRepository
    {
        public CourseVideoAttachmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseVideoAttachment>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(cva => cva.CourseVideo)
                .Include(cva => cva.FileTypeLookup)
                .Where(cva => !cva.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseVideoAttachment>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<CourseVideoAttachment>> GetByVideoIdAsync(Guid videoId)
        {
            return await _dbSet
                .Include(cva => cva.CourseVideo)
                .Include(cva => cva.FileTypeLookup)
                .Where(cva => cva.CourseVideoOid == videoId && !cva.IsDeleted)
                .ToListAsync();
        }
    }
}