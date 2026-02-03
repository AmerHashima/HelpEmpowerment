        using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseContentRepository : IRepository<CourseContent>
    {
        Task<PagedResult<CourseContent>> GetPagedAsync(DataRequest request);
        Task<List<CourseContent>> GetByOutlineIdAsync(Guid outlineId);
    }
}