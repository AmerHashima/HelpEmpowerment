using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IStudentCourseReservationRepository : IRepository<StudentCourseReservation>
    {
        Task<PagedResult<StudentCourseReservation>> GetPagedAsync(DataRequest request);
        Task<List<StudentCourseReservation>> GetByStudentCourseIdAsync(Guid studentCourseId);
        Task<StudentCourseReservation?> GetWithDetailsAsync(Guid id);
    }
}
