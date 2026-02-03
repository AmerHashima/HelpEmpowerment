using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class CourseQuestionRepository : Repository<CourseQuestion>, ICourseQuestionRepository
    {
        public CourseQuestionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<CourseQuestion>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(q => q.MasterExam)
                .Include(q => q.QuestionTypeLookup)
                .Include(q => q.Answers.Where(a => !a.IsDeleted))
                .Where(q => !q.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<CourseQuestion>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<CourseQuestion?> GetWithAnswersAsync(Guid id)
        {
            return await _dbSet
                .Include(q => q.MasterExam)
                .Include(q => q.QuestionTypeLookup)
                .Include(q => q.Answers.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(q => q.Oid == id && !q.IsDeleted);
        }

        public async Task<List<CourseQuestion>> GetByMasterExamIdAsync(Guid masterExamId)
        {
            return await _dbSet
                .Include(q => q.MasterExam)
                .Include(q => q.QuestionTypeLookup)
                .Include(q => q.Answers.Where(a => !a.IsDeleted))
                .Where(q => q.CoursesMasterExamOid == masterExamId && !q.IsDeleted && q.IsActive)
                .OrderBy(q => q.OrderNo)
                .ToListAsync();
        }

        public async Task<List<CourseQuestion>> GetByExamIdAsync(Guid examId)
        {
            return await _dbSet
                .Include(q => q.MasterExam)
                .Include(q => q.QuestionTypeLookup)
                .Where(q => q.CoursesMasterExamOid == examId && !q.IsDeleted && q.IsActive)
                .OrderBy(q => q.OrderNo)
                .ToListAsync();
        }

        public async Task<List<CourseQuestion>> GetWithAnswersByExamIdAsync(Guid examId)
        {
            return await _dbSet
                .Include(q => q.MasterExam)
                .Include(q => q.QuestionTypeLookup)
                .Include(q => q.Answers.Where(a => !a.IsDeleted))
                    .ThenInclude(a => a.Question)
                .Where(q => q.CoursesMasterExamOid == examId && !q.IsDeleted && q.IsActive)
                .OrderBy(q => q.OrderNo)
                .ToListAsync();
        }
    }
}