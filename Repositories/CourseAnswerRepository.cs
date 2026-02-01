using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Extensions;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseAnswerRepository : Repository<CourseAnswer>, ICourseAnswerRepository
    {
        public CourseAnswerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseAnswer>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(a => a.Question)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseAnswer>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<IEnumerable<CourseAnswer>> GetByQuestionIdAsync(Guid questionId)
        {
            return await _dbSet
                .Where(a => a.QuestionId == questionId && !a.IsDeleted)
                .OrderBy(a => a.OrderNo)
                .ToListAsync();
        }

        public async Task<bool> DeleteByQuestionIdAsync(Guid questionId)
        {
            var answers = await _dbSet
                .Where(a => a.QuestionId == questionId && !a.IsDeleted)
                .ToListAsync();

            foreach (var answer in answers)
            {
                answer.IsDeleted = true;
                answer.DeletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}