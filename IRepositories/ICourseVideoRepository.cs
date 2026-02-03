using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseVideoRepository : IRepository<CourseVideo>
    {
        Task<PagedResult<CourseVideo>> GetPagedAsync(DataRequest request);
        Task<List<CourseVideo>> GetByCourseIdAsync(Guid courseId);
        Task<CourseVideo?> GetWithAttachmentsAsync(Guid id);
    }
}