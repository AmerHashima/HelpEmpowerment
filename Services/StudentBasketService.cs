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

                decimal reservPrice = 0;
                if (dto.RecordedCourseReserv) reservPrice += course.RecordedCourseReservPrice ?? 0;
                if (dto.ExamSimulationReserv) reservPrice += course.ExamSimulationReservPrice ?? 0;
                if (dto.LiveCourseReserv) reservPrice += course.LiveCourseReservPrice ?? 0;

                var totalPrice = price + reservPrice;
                decimal discountAmount = 0;
                string? appliedCoupon = null;

                // Apply coupon if provided
                if (!string.IsNullOrEmpty(dto.CouponCode))
                {
                    var promoOwner = await _studentRepository.GetByPromoCodeAsync(dto.CouponCode);

                    if (promoOwner == null)
                        return ApiResponse<StudentBasketDto>.ErrorResponse("Invalid coupon code");

                    if (promoOwner.PromoToDateValid.HasValue && promoOwner.PromoToDateValid.Value < DateTime.UtcNow)
                        return ApiResponse<StudentBasketDto>.ErrorResponse("Coupon code has expired");

                    var discountPercent = (decimal)(promoOwner.PromoDiscount ?? 0);
                    discountAmount = totalPrice * (discountPercent / 100);
                    appliedCoupon = dto.CouponCode;
                }

                var entity = new StudentBasket
                {
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId,
                    OriginalPrice = totalPrice,
                    DiscountAmount = discountAmount,
                    FinalPrice = totalPrice - discountAmount,
                    CouponCode = appliedCoupon,
                    Quantity = 1,
                    RecordedCourseReserv = dto.RecordedCourseReserv,
                    LiveCourseReserv = dto.LiveCourseReserv,
                    ExamSimulationReserv = dto.ExamSimulationReserv,
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

                // Load course to recalculate price from live data
                var course = await _courseRepository.GetByIdAsync(entity.CourseId);
                if (course == null)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Course not found");

                decimal basePrice = 0;
                if (!string.IsNullOrEmpty(course.Price) && decimal.TryParse(course.Price, out var parsedPrice))
                    basePrice = parsedPrice;

                decimal reservPrice = 0;
                if (dto.RecordedCourseReserv) reservPrice += course.RecordedCourseReservPrice ?? 0;
                if (dto.ExamSimulationReserv) reservPrice += course.ExamSimulationReservPrice ?? 0;
                if (dto.LiveCourseReserv) reservPrice += course.LiveCourseReservPrice ?? 0;

                var totalPrice = basePrice + reservPrice;

                entity.Quantity = dto.Quantity;
                entity.RecordedCourseReserv = dto.RecordedCourseReserv;
                entity.LiveCourseReserv = dto.LiveCourseReserv;
                entity.ExamSimulationReserv = dto.ExamSimulationReserv;
                entity.OriginalPrice = totalPrice;
                entity.DiscountAmount = 0;
                entity.FinalPrice = totalPrice;
                entity.CouponCode = null;
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

        public async Task<ApiResponse<BasketSummaryDto>> ApplyCouponAsync(Guid studentId, string couponCode)
        {
            try
            {
                // Validate coupon against student promo table
                var promoOwner = await _studentRepository.GetByPromoCodeAsync(couponCode);

                if (promoOwner == null)
                    return ApiResponse<BasketSummaryDto>.ErrorResponse("Invalid coupon code");

                if (promoOwner.PromoToDateValid.HasValue && promoOwner.PromoToDateValid.Value < DateTime.UtcNow)
                    return ApiResponse<BasketSummaryDto>.ErrorResponse("Coupon code has expired");

                var discountPercent = (decimal)(promoOwner.PromoDiscount ?? 0);

                // Load basket items — do NOT update DB; coupon may be removed before checkout
                var items = await _repository.GetByStudentIdAsync(studentId);
                if (!items.Any())
                    return ApiResponse<BasketSummaryDto>.ErrorResponse("Basket is empty");

                // Calculate discounts in-memory only
                var dtos = items.Select(item =>
                {
                    var dto = MapToDto(item);
                    dto.CouponCode = couponCode;
                    dto.DiscountAmount = item.OriginalPrice * (discountPercent / 100);
                    dto.FinalPrice = item.OriginalPrice - (dto.DiscountAmount ?? 0);
                    return dto;
                }).ToList();

                var summary = new BasketSummaryDto
                {
                    Items = dtos,
                    SubTotal = dtos.Sum(i => i.OriginalPrice * i.Quantity),
                    TotalDiscount = dtos.Sum(i => (i.DiscountAmount ?? 0) * i.Quantity),
                    Total = dtos.Sum(i => i.FinalPrice * i.Quantity),
                    ItemCount = dtos.Sum(i => i.Quantity)
                };

                return ApiResponse<BasketSummaryDto>.SuccessResponse(summary, $"Coupon preview: {discountPercent}% discount applied (not saved — confirm at checkout)");
            }
            catch (Exception ex)
            {
                return ApiResponse<BasketSummaryDto>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<StudentCourseDto>>> CheckoutAsync(Guid studentId, string paymentMethod, string? couponCode = null)
        {
            try
            {
                var basketItems = await _repository.GetByStudentIdAsync(studentId);
                if (!basketItems.Any())
                    return ApiResponse<List<StudentCourseDto>>.ErrorResponse("Basket is empty");

                // Validate coupon at checkout time (not from stored values)
                decimal discountPercent = 0;
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var promoOwner = await _studentRepository.GetByPromoCodeAsync(couponCode);

                    if (promoOwner == null)
                        return ApiResponse<List<StudentCourseDto>>.ErrorResponse("Invalid coupon code");

                    if (promoOwner.PromoToDateValid.HasValue && promoOwner.PromoToDateValid.Value < DateTime.UtcNow)
                        return ApiResponse<List<StudentCourseDto>>.ErrorResponse("Coupon code has expired");

                    discountPercent = (decimal)(promoOwner.PromoDiscount ?? 0);
                }

                var enrollments = new List<StudentCourseDto>();

                foreach (var item in basketItems)
                {
                    // Recalculate from live course data — never trust stored discount
                    var course = await _courseRepository.GetByIdAsync(item.CourseId);

                    decimal basePrice = 0;
                    if (course != null && !string.IsNullOrEmpty(course.Price) && decimal.TryParse(course.Price, out var parsed))
                        basePrice = parsed;

                    decimal reservPrice = 0;
                    if (item.RecordedCourseReserv) reservPrice += course?.RecordedCourseReservPrice ?? 0;
                    if (item.ExamSimulationReserv) reservPrice += course?.ExamSimulationReservPrice ?? 0;
                    if (item.LiveCourseReserv) reservPrice += course?.LiveCourseReservPrice ?? 0;

                    var originalPrice = basePrice + reservPrice;
                    var discountAmount = originalPrice * (discountPercent / 100);
                    var paidAmount = originalPrice - discountAmount;

                    var enrollment = new StudentCourse
                    {
                        StudentId = studentId,
                        CourseId = item.CourseId,
                        Price = originalPrice,
                        DiscountAmount = discountAmount,
                        PaidAmount = paidAmount,
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
                RecordedCourseReserv = entity.RecordedCourseReserv,
                LiveCourseReserv = entity.LiveCourseReserv,
                ExamSimulationReserv = entity.ExamSimulationReserv,
                RecordedCourseReservPrice = entity.Course?.RecordedCourseReservPrice,
                ExamSimulationReservPrice = entity.Course?.ExamSimulationReservPrice,
                LiveCourseReservPrice = entity.Course?.LiveCourseReservPrice,
                AddedAt = entity.AddedAt
            };
        }
    }
}