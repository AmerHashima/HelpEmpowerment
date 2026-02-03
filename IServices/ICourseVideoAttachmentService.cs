using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseVideoAttachmentService
    {
        Task<PagedResponse<CourseVideoAttachmentDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseVideoAttachmentDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseVideoAttachmentDto>>> GetByVideoIdAsync(Guid videoId);
        Task<ApiResponse<CourseVideoAttachmentDto>> CreateAsync(CreateCourseVideoAttachmentDto dto);
        Task<ApiResponse<CourseVideoAttachmentDto>> UpdateAsync(UpdateCourseVideoAttachmentDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}