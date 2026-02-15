using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentBasketsController : ControllerBase
    {
        private readonly IStudentBasketService _service;

        public StudentBasketsController(IStudentBasketService service)
        {
            _service = service;
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<ApiResponse<BasketSummaryDto>>> GetBasket(Guid studentId)
        {
            var response = await _service.GetBasketAsync(studentId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{studentId}/count")]
        public async Task<ActionResult<ApiResponse<int>>> GetBasketCount(Guid studentId)
        {
            var response = await _service.GetBasketCountAsync(studentId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudentBasketDto>>> AddToBasket([FromBody] AddToBasketDto dto)
        {
            var response = await _service.AddToBasketAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<StudentBasketDto>>> UpdateBasketItem([FromBody] UpdateBasketDto dto)
        {
            var response = await _service.UpdateBasketItemAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveFromBasket(Guid id)
        {
            var response = await _service.RemoveFromBasketAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{studentId}/clear")]
        public async Task<ActionResult<ApiResponse<bool>>> ClearBasket(Guid studentId)
        {
            var response = await _service.ClearBasketAsync(studentId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{basketId}/coupon")]
        public async Task<ActionResult<ApiResponse<StudentBasketDto>>> ApplyCoupon(Guid basketId, [FromBody] ApplyCouponRequest request)
        {
            var response = await _service.ApplyCouponAsync(basketId, request.CouponCode);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{studentId}/checkout")]
        public async Task<ActionResult<ApiResponse<List<StudentCourseDto>>>> Checkout(Guid studentId, [FromBody] CheckoutRequest request)
        {
            var response = await _service.CheckoutAsync(studentId, request.PaymentMethod);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }

    public class ApplyCouponRequest
    {
        public string CouponCode { get; set; } = string.Empty;
    }

    public class CheckoutRequest
    {
        public string PaymentMethod { get; set; } = string.Empty;
    }
}