using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IStudentCourseRepository : IRepository<StudentCourse>
    {
        Task<PagedResult<StudentCourse>> GetPagedAsync(DataRequest request);
        Task<List<StudentCourse>> GetByStudentIdAsync(Guid studentId);
        Task<List<StudentCourse>> GetByCourseIdAsync(Guid courseId);
        Task<StudentCourse?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
        Task<StudentCourse?> GetWithDetailsAsync(Guid id);
        Task<List<StudentCourse>> GetByPaymentStatusAsync(Guid paymentStatusLookupId);
        Task<List<StudentCourse>> GetByEnrollmentStatusAsync(Guid enrollmentStatusLookupId);
        Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId);
    }
}