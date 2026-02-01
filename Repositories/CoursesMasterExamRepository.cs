using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Extensions;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class CoursesMasterExamRepository : Repository<CoursesMasterExam>, ICoursesMasterExamRepository
    {
        public CoursesMasterExamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CoursesMasterExam>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(e => e.Course)
                .Include(e => e.CourseLevelLookup)
                .Include(e => e.CourseCategoryLookup)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CoursesMasterExam>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<IEnumerable<CoursesMasterExam>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(e => e.CourseLevelLookup)
                .Include(e => e.CourseCategoryLookup)
                .Where(e => e.CourseOid == courseId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<CoursesMasterExam?> GetWithQuestionsAsync(Guid id)
        {
            return await _dbSet
                .Include(e => e.Course)
                .Include(e => e.CourseLevelLookup)
                .Include(e => e.CourseCategoryLookup)
                .Include(e => e.Questions.Where(q => !q.IsDeleted))
                    .ThenInclude(q => q.Answers.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(e => e.Oid == id && !e.IsDeleted);
        }
    }
}