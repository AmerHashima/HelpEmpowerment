using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

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
                    .ThenInclude(q => q.MasterExam)
                .Where(a => !a.IsDeleted)
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

        public async Task<List<CourseAnswer>> GetByQuestionIdAsync(Guid questionId)
        {
            return await _dbSet
                .Include(a => a.Question)
                .Where(a => a.QuestionId == questionId && !a.IsDeleted)
                .OrderBy(a => a.OrderNo)
                .ToListAsync();
        }

        public async Task<CourseAnswer?> GetCorrectAnswerByQuestionIdAsync(Guid questionId)
        {
            return await _dbSet
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.QuestionId == questionId && a.IsCorrect && !a.IsDeleted);
        }
    }
}