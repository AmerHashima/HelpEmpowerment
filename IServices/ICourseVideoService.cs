using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using Microsoft.AspNetCore.Http;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseVideoService
    {
        Task<PagedResponse<CourseVideoDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseVideoDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseVideoDto>>> GetByCourseIdAsync(Guid courseId);
        Task<ApiResponse<CourseVideoDto>> GetWithAttachmentsAsync(Guid id);
        Task<ApiResponse<CourseVideoDto>> CreateAsync(CreateCourseVideoDto dto);
        Task<ApiResponse<CourseVideoDto>> UpdateAsync(UpdateCourseVideoDto dto);
        Task<ApiResponse<CourseVideoDto>> UploadVideoAsync(Guid courseVideoId, IFormFile video, string savePath);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
