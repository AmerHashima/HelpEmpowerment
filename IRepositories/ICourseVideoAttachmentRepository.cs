using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface ICourseVideoAttachmentRepository : IRepository<CourseVideoAttachment>
    {
        Task<PagedResult<CourseVideoAttachment>> GetPagedAsync(DataRequest request);
        Task<List<CourseVideoAttachment>> GetByVideoIdAsync(Guid videoId);
    }
}