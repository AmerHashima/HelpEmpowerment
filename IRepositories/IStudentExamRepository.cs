using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IStudentExamRepository : IRepository<StudentExam>
    {
        Task<PagedResult<StudentExam>> GetPagedAsync(DataRequest request);
        Task<List<StudentExam>> GetByStudentIdAsync(Guid studentId);
        Task<List<StudentExam>> GetByMasterExamIdAsync(Guid masterExamId);
        Task<StudentExam?> GetWithQuestionsAsync(Guid id);
        Task<int> GetAttemptCountAsync(Guid studentId, Guid masterExamId);
    }
}