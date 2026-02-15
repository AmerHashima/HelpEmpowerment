using Microsoft.EntityFrameworkCore;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Repositories
{
    public class StudentBasketRepository : Repository<StudentBasket>, IStudentBasketRepository
    {
        public StudentBasketRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<StudentBasket>> GetByStudentIdAsync(Guid studentId)
        {
            return await _dbSet
                .Include(sb => sb.Student)
                .Include(sb => sb.Course)
                .Include(sb => sb.BasketStatus)
                .Where(sb => sb.StudentId == studentId && !sb.IsDeleted)
                .OrderByDescending(sb => sb.AddedAt)
                .ToListAsync();
        }

        public async Task<StudentBasket?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
        {
            return await _dbSet
                .Include(sb => sb.Course)
                .FirstOrDefaultAsync(sb => 
                    sb.StudentId == studentId && 
                    sb.CourseId == courseId && 
                    !sb.IsDeleted);
        }

        public async Task<StudentBasket?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(sb => sb.Student)
                .Include(sb => sb.Course)
                .Include(sb => sb.BasketStatus)
                .FirstOrDefaultAsync(sb => sb.Oid == id && !sb.IsDeleted);
        }

        public async Task<int> GetBasketItemCountAsync(Guid studentId)
        {
            return await _dbSet
                .Where(sb => sb.StudentId == studentId && !sb.IsDeleted)
                .SumAsync(sb => sb.Quantity);
        }

        public async Task<decimal> GetBasketTotalAsync(Guid studentId)
        {
            return await _dbSet
                .Where(sb => sb.StudentId == studentId && !sb.IsDeleted)
                .SumAsync(sb => sb.FinalPrice * sb.Quantity);
        }

        public async Task ClearBasketAsync(Guid studentId)
        {
            var items = await _dbSet
                .Where(sb => sb.StudentId == studentId && !sb.IsDeleted)
                .ToListAsync();

            foreach (var item in items)
            {
                item.IsDeleted = true;
                item.DeletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}