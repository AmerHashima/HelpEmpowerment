using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class StudentExamQuestionAnswerRepository : Repository<StudentExamQuestionAnswer>, IStudentExamQuestionAnswerRepository
    {
        public StudentExamQuestionAnswerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<StudentExamQuestionAnswer>> GetByStudentExamQuestionIdAsync(Guid studentExamQuestionId)
        {
            return await _dbSet
                .Include(a => a.SelectedAnswer)
                .Include(a => a.AnswerSelectedAnswer)
                .Where(a => a.StudentExamQuestionOid == studentExamQuestionId && !a.IsDeleted)
                .ToListAsync();
        }
    }
}
