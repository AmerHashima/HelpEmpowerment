using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Extensions;

namespace HelpEmpowermentApi.Repositories
{
    public class ServiceContactUsRepository : Repository<ServiceContactUs>, IServiceContactUsRepository
    {
        public ServiceContactUsRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PagedResult<ServiceContactUs>> GetPagedAsync(DataRequest request)
        {
            var query = _dbSet
                .Include(c => c.ContactType)
                .Include(c => c.Priority)
                .Include(c => c.Status)
                .Include(c => c.Student)
                .Include(c => c.User)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            query = query.ApplyFilters(request.Filters);
            var totalCount = await query.CountAsync();

            query = query.ApplySorting(request.Sort);
            query = query.ApplyPagination(request.Pagination);

            var items = await query.ToListAsync();

            return new PagedResult<ServiceContactUs>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize
            };
        }

        public async Task<ServiceContactUs?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.ContactType)
                .Include(c => c.Priority)
                .Include(c => c.Status)
                .Include(c => c.Student)
                .Include(c => c.User)
                .Include(c => c.Responder)
                .FirstOrDefaultAsync(c => c.Oid == id && !c.IsDeleted);
        }

        public async Task<ServiceContactUs?> GetByTicketNumberAsync(string ticketNumber)
        {
            return await _dbSet
                .Include(c => c.ContactType)
                .Include(c => c.Priority)
                .Include(c => c.Status)
                .FirstOrDefaultAsync(c => c.TicketNumber == ticketNumber && !c.IsDeleted);
        }

        public async Task<List<ServiceContactUs>> GetByStatusAsync(Guid statusLookupId)
        {
            return await _dbSet
                .Include(c => c.ContactType)
                .Include(c => c.Priority)
                .Include(c => c.Status)
                .Where(c => c.StatusLookupId == statusLookupId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ServiceContactUs>> GetByStudentIdAsync(Guid studentId)
        {
            return await _dbSet
                .Include(c => c.ContactType)
                .Include(c => c.Status)
                .Where(c => c.StudentId == studentId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ServiceContactUs>> GetUnreadAsync()
        {
            return await _dbSet
                .Include(c => c.ContactType)
                .Include(c => c.Priority)
                .Where(c => !c.IsRead && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _dbSet.CountAsync(c => !c.IsRead && !c.IsDeleted);
        }

        public async Task<string> GenerateTicketNumberAsync()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var lastTicket = await _dbSet
                .Where(c => c.TicketNumber != null && c.TicketNumber.StartsWith($"TKT-{date}"))
                .OrderByDescending(c => c.TicketNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastTicket?.TicketNumber != null)
            {
                var parts = lastTicket.TicketNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastSequence))
                {
                    sequence = lastSequence + 1;
                }
            }

            return $"TKT-{date}-{sequence:D4}";
        }
    }
}