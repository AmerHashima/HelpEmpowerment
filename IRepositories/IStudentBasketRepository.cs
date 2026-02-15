using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.IRepositories
{
    public interface IStudentBasketRepository : IRepository<StudentBasket>
    {
        Task<List<StudentBasket>> GetByStudentIdAsync(Guid studentId);
        Task<StudentBasket?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
        Task<StudentBasket?> GetWithDetailsAsync(Guid id);
        Task<int> GetBasketItemCountAsync(Guid studentId);
        Task<decimal> GetBasketTotalAsync(Guid studentId);
        Task ClearBasketAsync(Guid studentId);
    }
}