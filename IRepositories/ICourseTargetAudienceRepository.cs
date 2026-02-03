using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseTargetAudienceRepository : IRepository<CourseTargetAudience>
    {
        Task<PagedResult<CourseTargetAudience>> GetPagedAsync(DataRequest request);
        Task<List<CourseTargetAudience>> GetByCourseIdAsync(Guid courseId);
    }
}