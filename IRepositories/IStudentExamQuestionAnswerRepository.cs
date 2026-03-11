using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IStudentExamQuestionAnswerRepository : IRepository<StudentExamQuestionAnswer>
    {
        Task<List<StudentExamQuestionAnswer>> GetByStudentExamQuestionIdAsync(Guid studentExamQuestionId);
    }
}
