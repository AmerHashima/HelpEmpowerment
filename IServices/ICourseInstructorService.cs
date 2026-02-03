using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseInstructorService
    {
        Task<PagedResponse<CourseInstructorDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseInstructorDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseInstructorDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseInstructorDto>> CreateAsync(CreateCourseInstructorDto dto);
        Task<ApiResponse<CourseInstructorDto>> UpdateAsync(UpdateCourseInstructorDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}