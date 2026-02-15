using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class StudentBasketService : IStudentBasketService
    {
        private readonly IStudentBasketRepository _repository;
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;

        public StudentBasketService(
            IStudentBasketRepository repository,
            IStudentCourseRepository studentCourseRepository,
            ICourseRepository courseRepository,
            IStudentRepository studentRepository)
        {
            _repository = repository;
            _studentCourseRepository = studentCourseRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
        }

        public async Task<ApiResponse<StudentBasketDto>> AddToBasketAsync(AddToBasketDto dto)
        {
            try
            {
                // Check if student exists
                var student = await _studentRepository.GetByIdAsync(dto.StudentId);
                if (student == null)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Student not found");

                // Check if course exists
                var course = await _courseRepository.GetByIdAsync(dto.CourseId);
                if (course == null)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Course not found");

                // Check if already enrolled
                var isEnrolled = await _studentCourseRepository.IsStudentEnrolledAsync(dto.StudentId, dto.CourseId);
                if (isEnrolled)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("You are already enrolled in this course");

                // Check if already in basket
                var existingItem = await _repository.GetByStudentAndCourseAsync(dto.StudentId, dto.CourseId);
                if (existingItem != null)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Course is already in your basket");

                decimal? coursePrice = null;
                if (!string.IsNullOrEmpty(course.Price) && decimal.TryParse(course.Price, out var parsedPrice))
                {
                    coursePrice = parsedPrice;
                }

                var price = coursePrice ?? 0;
                decimal discountAmount = 0;

                // Apply coupon if provided
                if (!string.IsNullOrEmpty(dto.CouponCode))
                {
                    // TODO: Validate coupon and calculate discount
                    discountAmount = price * 0.1m; // Example: 10% discount
                }

                var entity = new StudentBasket
                {
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId,
                    OriginalPrice = price,
                    DiscountAmount = discountAmount,
                    FinalPrice = price - discountAmount,
                    CouponCode = dto.CouponCode,
                    Quantity = 1,
                    AddedAt = DateTime.UtcNow,
                    CreatedBy = dto.StudentId,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(entity);
                var result = await _repository.GetWithDetailsAsync(created.Oid);
                return ApiResponse<StudentBasketDto>.SuccessResponse(MapToDto(result!), "Course added to basket");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentBasketDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentBasketDto>> UpdateBasketItemAsync(UpdateBasketDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(dto.Oid);
                if (entity == null)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Basket item not found");

                entity.Quantity = dto.Quantity;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                var result = await _repository.GetWithDetailsAsync(dto.Oid);
                return ApiResponse<StudentBasketDto>.SuccessResponse(MapToDto(result!));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentBasketDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RemoveFromBasketAsync(Guid id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Basket item not found");

                return ApiResponse<bool>.SuccessResponse(true, "Item removed from basket");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<BasketSummaryDto>> GetBasketAsync(Guid studentId)
        {
            try
            {
                var items = await _repository.GetByStudentIdAsync(studentId);
                var dtos = items.Select(MapToDto).ToList();

                var summary = new BasketSummaryDto
                {
                    Items = dtos,
                    SubTotal = items.Sum(i => i.OriginalPrice * i.Quantity),
                    TotalDiscount = items.Sum(i => (i.DiscountAmount ?? 0) * i.Quantity),
                    Total = items.Sum(i => i.FinalPrice * i.Quantity),
                    ItemCount = items.Sum(i => i.Quantity)
                };

                return ApiResponse<BasketSummaryDto>.SuccessResponse(summary);
            }
            catch (Exception ex)
            {
                return ApiResponse<BasketSummaryDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> GetBasketCountAsync(Guid studentId)
        {
            try
            {
                var count = await _repository.GetBasketItemCountAsync(studentId);
                return ApiResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ClearBasketAsync(Guid studentId)
        {
            try
            {
                await _repository.ClearBasketAsync(studentId);
                return ApiResponse<bool>.SuccessResponse(true, "Basket cleared");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentBasketDto>> ApplyCouponAsync(Guid basketId, string couponCode)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(basketId);
                if (entity == null)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Basket item not found");

                // TODO: Validate coupon
                var discount = entity.OriginalPrice * 0.1m; // Example: 10% discount

                entity.CouponCode = couponCode;
                entity.DiscountAmount = discount;
                entity.FinalPrice = entity.OriginalPrice - discount;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(entity);
                var result = await _repository.GetWithDetailsAsync(basketId);
                return ApiResponse<StudentBasketDto>.SuccessResponse(MapToDto(result!), "Coupon applied");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentBasketDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<StudentCourseDto>>> CheckoutAsync(Guid studentId, string paymentMethod)
        {
            try
            {
                var basketItems = await _repository.GetByStudentIdAsync(studentId);
                if (!basketItems.Any())
                    return ApiResponse<List<StudentCourseDto>>.ErrorResponse("Basket is empty");

                var enrollments = new List<StudentCourseDto>();

                foreach (var item in basketItems)
                {
                    // Create enrollment
                    var enrollment = new StudentCourse
                    {
                        StudentId = studentId,
                        CourseId = item.CourseId,
                        Price = item.OriginalPrice,
                        DiscountAmount = item.DiscountAmount,
                        PaidAmount = item.FinalPrice,
                        PaymentMethod = paymentMethod,
                        PaymentDate = DateTime.UtcNow,
                        EnrollmentDate = DateTime.UtcNow,
                        CreatedBy = studentId,
                        CreatedAt = DateTime.UtcNow
                    };

                    var created = await _studentCourseRepository.AddAsync(enrollment);
                    
                    enrollments.Add(new StudentCourseDto
                    {
                        Oid = created.Oid,
                        StudentId = created.StudentId,
                        CourseId = created.CourseId,
                        Price = created.Price,
                        PaidAmount = created.PaidAmount,
                        PaymentMethod = created.PaymentMethod,
                        EnrollmentDate = created.EnrollmentDate
                    });
                }

                // Clear basket after checkout
                await _repository.ClearBasketAsync(studentId);

                return ApiResponse<List<StudentCourseDto>>.SuccessResponse(enrollments, "Checkout successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<StudentCourseDto>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static StudentBasketDto MapToDto(StudentBasket entity)
        {
            return new StudentBasketDto
            {
                Oid = entity.Oid,
                StudentId = entity.StudentId,
                StudentName = entity.Student?.NameEn,
                CourseId = entity.CourseId,
                CourseName = entity.Course?.CourseName,
               // CourseImage = entity.Course?.ImageUrl,
                OriginalPrice = entity.OriginalPrice,
                DiscountAmount = entity.DiscountAmount,
                FinalPrice = entity.FinalPrice,
                CouponCode = entity.CouponCode,
                Quantity = entity.Quantity,
                AddedAt = entity.AddedAt
            };
        }
    }
}