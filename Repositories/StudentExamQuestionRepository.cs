using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class StudentExamQuestionRepository : Repository<StudentExamQuestion>, IStudentExamQuestionRepository
    {
        public StudentExamQuestionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<StudentExamQuestion>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(seq => seq.StudentExam)
                .Include(seq => seq.Question)
                .Include(seq => seq.SelectedAnswer)
                .Where(seq => !seq.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<StudentExamQuestion>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<List<StudentExamQuestion>> GetByStudentExamIdAsync(Guid studentExamId)
        {
            return await _dbSet
                .Include(seq => seq.StudentExam)
                .Include(seq => seq.Question)
                .Include(seq => seq.SelectedAnswer)
                .Where(seq => seq.StudentExamOid == studentExamId && !seq.IsDeleted)
                .ToListAsync();
        }
    }
}