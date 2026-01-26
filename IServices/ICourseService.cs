using StandardArticture.Common;
using StandardArticture.DTOs;

namespace StandardArticture.IServices
{
    public interface ICourseService
    {
        Task<PagedResponse<CourseDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<CourseDto>> GetByCodeAsync(string courseCode);
        Task<ApiResponse<CourseDto>> CreateAsync(CreateCourseDto dto);
        Task<ApiResponse<CourseDto>> UpdateAsync(UpdateCourseDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}