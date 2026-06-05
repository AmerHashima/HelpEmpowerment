using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using Microsoft.AspNetCore.Http;

namespace HelpEmpowermentApi.IServices
{
    public interface ICourseVideoAttachmentService
    {
        Task<PagedResponse<CourseVideoAttachmentDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<CourseVideoAttachmentDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<CourseVideoAttachmentDto>>> GetByVideoIdAsync(Guid videoId);
        Task<ApiResponse<CourseVideoAttachmentDto>> CreateAsync(CreateCourseVideoAttachmentDto dto);
        Task<ApiResponse<CourseVideoAttachmentDto>> UploadAsync(Guid courseVideoOid, IFormFile file, string savePath, Guid? fileTypeLookupId);
        Task<ApiResponse<CourseVideoAttachmentDto>> UpdateAsync(UpdateCourseVideoAttachmentDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
