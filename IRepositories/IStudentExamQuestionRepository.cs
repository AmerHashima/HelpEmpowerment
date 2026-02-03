using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IStudentExamQuestionRepository : IRepository<StudentExamQuestion>
    {
        Task<PagedResult<StudentExamQuestion>> GetPagedAsync(DataRequest request);
        Task<List<StudentExamQuestion>> GetByStudentExamIdAsync(Guid studentExamId);
    }
}