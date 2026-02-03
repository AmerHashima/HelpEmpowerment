using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IStudentService
    {
        Task<PagedResponse<StudentDto>> GetPagedAsync(DataRequest request);
        Task<ApiResponse<StudentDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<StudentDto>> GetByUsernameAsync(string username);
        Task<ApiResponse<StudentDto>> CreateAsync(CreateStudentDto dto);
        Task<ApiResponse<StudentDto>> UpdateAsync(UpdateStudentDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<StudentDto>> AuthenticateAsync(string username, string password);
    }
}