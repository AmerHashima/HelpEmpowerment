using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IServiceContactUsRepository : IRepository<ServiceContactUs>
    {
        Task<PagedResult<ServiceContactUs>> GetPagedAsync(DataRequest request);
        Task<ServiceContactUs?> GetWithDetailsAsync(Guid id);
        Task<ServiceContactUs?> GetByTicketNumberAsync(string ticketNumber);
        Task<List<ServiceContactUs>> GetByStatusAsync(Guid statusLookupId);
        Task<List<ServiceContactUs>> GetByStudentIdAsync(Guid studentId);
        Task<List<ServiceContactUs>> GetUnreadAsync();
        Task<int> GetUnreadCountAsync();
        Task<string> GenerateTicketNumberAsync();
    }
}