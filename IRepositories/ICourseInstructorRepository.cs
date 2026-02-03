using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseInstructorRepository : IRepository<CourseInstructor>
    {
        Task<PagedResult<CourseInstructor>> GetPagedAsync(DataRequest request);
        Task<List<CourseInstructor>> GetByCourseIdAsync(Guid courseId);
    }
}