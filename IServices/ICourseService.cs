using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseService
    {
        Task<PagedResponse<CourseDto>> GetPagedAsync(DataRequest request, Guid? assignedUserId = null);
        Task<ApiResponse<CourseDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<CourseDto>> GetByCodeAsync(string courseCode);
        Task<ApiResponse<CourseDto>> CreateAsync(CreateCourseDto dto);
        Task<ApiResponse<CourseDto>> UpdateAsync(UpdateCourseDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<CourseDto>> UploadImageAsync(Guid id, IFormFile image);
        Task<ApiResponse<string>> GetImagePathAsync(Guid id);
        Task<ApiResponse<bool>> DeleteImageAsync(Guid id);
    }
}
