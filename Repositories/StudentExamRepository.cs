using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class StudentExamRepository : Repository<StudentExam>, IStudentExamRepository
    {
        public StudentExamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<StudentExam>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(se => se.Student)
                .Include(se => se.MasterExam)
                .Include(se => se.ExamStatusLookup)
                .Where(se => !se.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<StudentExam>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<StudentExam>> GetByStudentIdAsync(Guid studentId)
        {
            return await _dbSet
                .Include(se => se.Student)
                .Include(se => se.MasterExam)
                .Include(se => se.ExamStatusLookup)
                .Where(se => se.StudentOid == studentId && !se.IsDeleted)
                .OrderByDescending(se => se.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<StudentExam>> GetByMasterExamIdAsync(Guid masterExamId)
        {
            return await _dbSet
                .Include(se => se.Student)
                .Include(se => se.MasterExam)
                .Include(se => se.ExamStatusLookup)
                .Where(se => se.CoursesMasterExamOid == masterExamId && !se.IsDeleted)
                .OrderByDescending(se => se.CreatedAt)
                .ToListAsync();
        }

        public async Task<StudentExam?> GetWithQuestionsAsync(Guid id)
        {
            return await _dbSet
                .Include(se => se.Student)
                .Include(se => se.MasterExam)
                .Include(se => se.ExamStatusLookup)
                .Include(se => se.ExamQuestions.Where(q => !q.IsDeleted))
                    .ThenInclude(eq => eq.Question)
                .Include(se => se.ExamQuestions.Where(q => !q.IsDeleted))
                    .ThenInclude(eq => eq.SelectedAnswer)
                .FirstOrDefaultAsync(se => se.Oid == id && !se.IsDeleted);
        }

        public async Task<int> GetAttemptCountAsync(Guid studentId, Guid masterExamId)
        {
            return await _dbSet
                .Where(se => se.StudentOid == studentId && se.CoursesMasterExamOid == masterExamId && !se.IsDeleted)
                .CountAsync();
        }
    }
}