using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IStudentCourseReservationService
    {
        Task<PagedResponse<StudentCourseReservationDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<StudentCourseReservationDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<StudentCourseReservationDto>>> GetByStudentCourseIdAsync(Guid studentCourseId);
        Task<ApiResponse<StudentCourseReservationDto>> CreateAsync(CreateStudentCourseReservationDto dto);
        Task<ApiResponse<StudentCourseReservationDto>> UpdateAsync(UpdateStudentCourseReservationDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
