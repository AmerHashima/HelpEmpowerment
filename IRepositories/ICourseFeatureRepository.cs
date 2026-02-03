using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseFeatureRepository : IRepository<CourseFeature>
    {
        Task<PagedResult<CourseFeature>> GetPagedAsync(DataRequest request);
        Task<List<CourseFeature>> GetByCourseIdAsync(Guid courseId);
    }
}