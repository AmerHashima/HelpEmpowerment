using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using Microsoft.AspNetCore.Http;

namespace HelpEmpowermentApi.IServices
{
    public interface IServiceContactUsService
    {
        Task<PagedResponse<ServiceContactUsDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<ServiceContactUsDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<ServiceContactUsDto>> GetByTicketNumberAsync(string ticketNumber);
        Task<ApiResponse<List<ServiceContactUsDto>>> GetByStudentIdAsync(Guid studentId);
        Task<ApiResponse<List<ServiceContactUsDto>>> GetUnreadAsync();
        Task<ApiResponse<int>> GetUnreadCountAsync();
        Task<ApiResponse<ServiceContactUsDto>> CreateAsync(CreateContactUsDto dto);
        Task<ApiResponse<ServiceContactUsDto>> RespondAsync(RespondContactUsDto dto);
        Task<ApiResponse<bool>> MarkAsReadAsync(Guid id, Guid readBy);
        Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, Guid statusLookupId);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<string>> UploadAttachmentAsync(Guid id, IFormFile file);
        Task<ApiResponse<string>> GetAttachmentFileNameAsync(Guid id);
        Task<ApiResponse<bool>> DeleteAttachmentAsync(Guid id);
    }
}