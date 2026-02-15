using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;

namespace HelpEmpowermentApi.IServices
{
    public interface IStudentBasketService
    {
        Task<ApiResponse<StudentBasketDto>> AddToBasketAsync(AddToBasketDto dto);
        Task<ApiResponse<StudentBasketDto>> UpdateBasketItemAsync(UpdateBasketDto dto);
        Task<ApiResponse<bool>> RemoveFromBasketAsync(Guid id);
        Task<ApiResponse<BasketSummaryDto>> GetBasketAsync(Guid studentId);
        Task<ApiResponse<int>> GetBasketCountAsync(Guid studentId);
        Task<ApiResponse<bool>> ClearBasketAsync(Guid studentId);
        Task<ApiResponse<StudentBasketDto>> ApplyCouponAsync(Guid basketId, string couponCode);
        Task<ApiResponse<List<StudentCourseDto>>> CheckoutAsync(Guid studentId, string paymentMethod);
    }
}