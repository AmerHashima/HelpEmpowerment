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
        private readonly IStudentCourseReservationRepository _studentCourseReservationRepository;
        private readonly ICourseServiceRepository _courseServiceRepository;

        public StudentBasketService(
            IStudentBasketRepository repository,
            IStudentCourseRepository studentCourseRepository,
            ICourseRepository courseRepository,
            IStudentRepository studentRepository,
            IStudentCourseReservationRepository studentCourseReservationRepository,
            ICourseServiceRepository courseServiceRepository)
        {
            _repository = repository;
            _studentCourseRepository = studentCourseRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _studentCourseReservationRepository = studentCourseReservationRepository;
            _courseServiceRepository = courseServiceRepository;
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

                var requestedAnyService = dto.ExamSimulationReserv || dto.RecordedCourseReserv || dto.LiveCourseReserv;

                // Check if already enrolled (allow buying additional services only)
                var isEnrolled = await _studentCourseRepository.IsStudentEnrolledAsync(dto.StudentId, dto.CourseId);
                if (isEnrolled && !requestedAnyService)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("You are already enrolled in this course");

                var existingItem = await _repository.GetByStudentAndCourseAsync(dto.StudentId, dto.CourseId);

                var mergedExamSimulation = existingItem?.ExamSimulationReserv == true || dto.ExamSimulationReserv;
                var mergedRecordedCourse = existingItem?.RecordedCourseReserv == true || dto.RecordedCourseReserv;
                var mergedLiveCourse = existingItem?.LiveCourseReserv == true || dto.LiveCourseReserv;
                var hasSelectedServices = mergedExamSimulation || mergedRecordedCourse || mergedLiveCourse;
                if (!hasSelectedServices)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Select at least one course service.");

                var existingServices = await GetAlreadyPurchasedServicesAsync(
                    dto.StudentId, dto.CourseId,
                    mergedExamSimulation, mergedRecordedCourse, mergedLiveCourse);
                if (existingServices.Count > 0)
                    return ApiResponse<StudentBasketDto>.ErrorResponse(
                        $"The following course service(s) are already reserved: {string.Join(", ", existingServices)}");

                var missingServices = await GetUnavailableServicesAsync(
                    dto.CourseId, mergedExamSimulation, mergedRecordedCourse, mergedLiveCourse);
                if (missingServices.Count > 0)
                    return ApiResponse<StudentBasketDto>.ErrorResponse(
                        $"The following course service(s) are not configured or inactive: {string.Join(", ", missingServices)}");

                var totalPrice = await GetSelectedServicesTotalPriceAsync(
                    dto.CourseId,
                    mergedExamSimulation,
                    mergedRecordedCourse,
                    mergedLiveCourse);
                if (totalPrice <= 0)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Selected services have invalid prices.");
                decimal discountAmount = 0;
                string? appliedCoupon = existingItem?.CouponCode;

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

                if (existingItem != null)
                {
                    var hasNewService =
                        (mergedExamSimulation != existingItem.ExamSimulationReserv)
                        || (mergedRecordedCourse != existingItem.RecordedCourseReserv)
                        || (mergedLiveCourse != existingItem.LiveCourseReserv);

                    if (!hasNewService)
                        return ApiResponse<StudentBasketDto>.ErrorResponse("Course is already in your basket");

                    existingItem.ExamSimulationReserv = mergedExamSimulation;
                    existingItem.RecordedCourseReserv = mergedRecordedCourse;
                    existingItem.LiveCourseReserv = mergedLiveCourse;
                    existingItem.OriginalPrice = totalPrice;
                    existingItem.DiscountAmount = discountAmount;
                    existingItem.FinalPrice = totalPrice - discountAmount;
                    existingItem.CouponCode = appliedCoupon;
                    existingItem.UpdatedAt = DateTime.UtcNow;

                    await _repository.UpdateAsync(existingItem);
                    var updated = await _repository.GetWithDetailsAsync(existingItem.Oid);
                    var updatedDto = MapToDto(updated!);
                    await PopulateServicePricesAsync(updatedDto);
                    return ApiResponse<StudentBasketDto>.SuccessResponse(updatedDto, "Course basket updated with selected service(s)");
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
                    RecordedCourseReserv = mergedRecordedCourse,
                    LiveCourseReserv = mergedLiveCourse,
                    ExamSimulationReserv = mergedExamSimulation,
                    AddedAt = DateTime.UtcNow,
                    CreatedBy = dto.StudentId,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(entity);
                var result = await _repository.GetWithDetailsAsync(created.Oid);
                var createdDto = MapToDto(result!);
                await PopulateServicePricesAsync(createdDto);
                return ApiResponse<StudentBasketDto>.SuccessResponse(createdDto, "Course added to basket");
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

                var hasSelectedServices = dto.ExamSimulationReserv || dto.RecordedCourseReserv || dto.LiveCourseReserv;
                if (!hasSelectedServices)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Select at least one course service.");

                var existingServices = await GetAlreadyPurchasedServicesAsync(
                    entity.StudentId, entity.CourseId,
                    dto.ExamSimulationReserv, dto.RecordedCourseReserv, dto.LiveCourseReserv);
                if (existingServices.Count > 0)
                    return ApiResponse<StudentBasketDto>.ErrorResponse(
                        $"The following course service(s) are already reserved: {string.Join(", ", existingServices)}");

                var missingServices = await GetUnavailableServicesAsync(
                    entity.CourseId, dto.ExamSimulationReserv, dto.RecordedCourseReserv, dto.LiveCourseReserv);
                if (missingServices.Count > 0)
                    return ApiResponse<StudentBasketDto>.ErrorResponse(
                        $"The following course service(s) are not configured or inactive: {string.Join(", ", missingServices)}");

                var totalPrice = await GetSelectedServicesTotalPriceAsync(
                    entity.CourseId,
                    dto.ExamSimulationReserv,
                    dto.RecordedCourseReserv,
                    dto.LiveCourseReserv);
                if (totalPrice <= 0)
                    return ApiResponse<StudentBasketDto>.ErrorResponse("Selected services have invalid prices.");

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
                var updatedDto = MapToDto(result!);
                await PopulateServicePricesAsync(updatedDto);
                return ApiResponse<StudentBasketDto>.SuccessResponse(updatedDto);
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
                await PopulateServicePricesAsync(dtos);

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
                await PopulateServicePricesAsync(dtos);

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

                var courseServicesByCourseId = new Dictionary<Guid, List<Models.CourseService>>();
                foreach (var courseId in basketItems.Select(item => item.CourseId).Distinct())
                {
                    courseServicesByCourseId[courseId] = await _courseServiceRepository.GetByCourseIdAsync(courseId);
                }

                foreach (var item in basketItems)
                {
                    var missingServices = GetMissingRequestedServiceValues(item, courseServicesByCourseId[item.CourseId]);
                    if (missingServices.Any())
                    {
                        return ApiResponse<List<StudentCourseDto>>.ErrorResponse(
                            $"Course service not configured for selected reservation(s): {string.Join(", ", missingServices)}");
                    }
                }

                var enrollments = new List<StudentCourseDto>();

                foreach (var item in basketItems)
                {
                    var requestedServiceValues = GetRequestedServiceValues(item).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    if (!requestedServiceValues.Any())
                        return ApiResponse<List<StudentCourseDto>>.ErrorResponse("Each basket item must include at least one selected service.");

                    var selectedServices = courseServicesByCourseId[item.CourseId]
                        .Where(service => service.IsActive
                            && !service.IsDeleted
                            && service.ServiceLookup?.LookupValue != null
                            && requestedServiceValues.Contains(service.ServiceLookup.LookupValue))
                        .ToList();

                    var originalPrice = selectedServices.Sum(service => service.Price);
                    if (originalPrice <= 0)
                        return ApiResponse<List<StudentCourseDto>>.ErrorResponse($"Selected services have invalid prices for course {item.CourseId}.");

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
                        RecordedCourseReserv = item.RecordedCourseReserv,
                        LiveCourseReserv = item.LiveCourseReserv,
                        ExamSimulationReserv = item.ExamSimulationReserv,
                        CreatedBy = studentId,
                        CreatedAt = DateTime.UtcNow
                    };

                    var created = await _studentCourseRepository.AddAsync(enrollment);
                    var createdReservations = await CreateReservationsFromBasketItemAsync(
                        created.Oid,
                        item,
                        courseServicesByCourseId[item.CourseId]);

                    enrollments.Add(new StudentCourseDto
                    {
                        Oid = created.Oid,
                        StudentId = created.StudentId,
                        CourseId = created.CourseId,
                        Price = created.Price,
                        DiscountAmount = created.DiscountAmount,
                        PaidAmount = created.PaidAmount,
                        PaymentMethod = created.PaymentMethod,
                        PaymentDate = created.PaymentDate,
                        EnrollmentDate = created.EnrollmentDate,
                        RecordedCourseReserv = created.RecordedCourseReserv,
                        LiveCourseReserv = created.LiveCourseReserv,
                        ExamSimulationReserv = created.ExamSimulationReserv,
                        Reservations = createdReservations
                    });
                }

                // Hard-delete basket rows after successful checkout.
                await _repository.HardClearBasketAsync(studentId);

                return ApiResponse<List<StudentCourseDto>>.SuccessResponse(enrollments, "Checkout successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<StudentCourseDto>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private async Task<List<StudentCourseReservationDto>> CreateReservationsFromBasketItemAsync(
            Guid studentCourseId,
            StudentBasket item,
            List<Models.CourseService> courseServices)
        {
            var requestedServiceValues = GetRequestedServiceValues(item).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!requestedServiceValues.Any())
                return new List<StudentCourseReservationDto>();

            var selectedServices = courseServices
                .Where(cs =>
                    cs.IsActive &&
                    cs.ServiceLookup?.LookupValue != null &&
                    requestedServiceValues.Contains(cs.ServiceLookup.LookupValue))
                .ToList();

            var reservations = new List<StudentCourseReservationDto>();

            foreach (var courseService in selectedServices)
            {
                var reservation = new StudentCourseReservation
                {
                    StudentCourseId = studentCourseId,
                    CourseServiceId = courseService.Oid,
                    ServicePrice = courseService.Price,
                    IsReserved = false,
                    CreatedBy = item.StudentId,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _studentCourseReservationRepository.AddAsync(reservation);

                reservations.Add(new StudentCourseReservationDto
                {
                    Oid = created.Oid,
                    StudentCourseId = created.StudentCourseId,
                    CourseServiceId = created.CourseServiceId,
                    CourseName = item.Course?.CourseName,
                    ServiceName = courseService.ServiceLookup?.LookupNameEn,
                    ServicePrice = created.ServicePrice,
                    ActiveTime = courseService.ActiveTime,
                    ReservationDate = created.ReservationDate,
                    ReservationExpiryDate = created.ReservationExpiryDate,
                    IsReserved = created.IsReserved,
                    Notes = created.Notes,
                    CreatedAt = created.CreatedAt,
                    CreatedBy = created.CreatedBy,
                    UpdatedAt = created.UpdatedAt,
                    UpdatedBy = created.UpdatedBy
                });
            }

            return reservations;
        }

        private static List<string> GetMissingRequestedServiceValues(StudentBasket item, List<Models.CourseService> courseServices)
        {
            var activeServiceValues = courseServices
                .Where(cs => cs.IsActive && cs.ServiceLookup?.LookupValue != null)
                .Select(cs => cs.ServiceLookup!.LookupValue)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return GetRequestedServiceValues(item)
                .Where(serviceValue => !activeServiceValues.Contains(serviceValue))
                .ToList();
        }

        private static IEnumerable<string> GetRequestedServiceValues(StudentBasket item)
        {
            if (item.ExamSimulationReserv)
                yield return "EXAM_SIMULATION";

            if (item.RecordedCourseReserv)
                yield return "RECORDED_COURSE";

            if (item.LiveCourseReserv)
                yield return "LIVE_COURSE";
        }

        private async Task<List<string>> GetAlreadyPurchasedServicesAsync(
            Guid studentId,
            Guid courseId,
            bool examSimulation,
            bool recordedCourse,
            bool liveCourse)
        {
            var requested = BuildRequestedServiceValues(examSimulation, recordedCourse, liveCourse);

            return await _studentCourseReservationRepository
                .GetExistingServiceValuesAsync(studentId, courseId, requested);
        }

        private async Task<List<string>> GetUnavailableServicesAsync(
            Guid courseId,
            bool examSimulation,
            bool recordedCourse,
            bool liveCourse)
        {
            var requested = BuildRequestedServiceValues(examSimulation, recordedCourse, liveCourse);
            if (requested.Count == 0) return [];

            var configured = (await _courseServiceRepository.GetByCourseIdAsync(courseId))
                .Where(service => service.IsActive && !service.IsDeleted && service.ServiceLookup != null)
                .Select(service => service.ServiceLookup!.LookupValue)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            return requested.Where(value => !configured.Contains(value)).ToList();
        }

        private async Task<decimal> GetSelectedServicesTotalPriceAsync(
            Guid courseId,
            bool examSimulation,
            bool recordedCourse,
            bool liveCourse)
        {
            var requested = BuildRequestedServiceValues(examSimulation, recordedCourse, liveCourse);
            if (requested.Count == 0)
                return 0;

            var configuredServices = (await _courseServiceRepository.GetByCourseIdAsync(courseId))
                .Where(service => service.IsActive && !service.IsDeleted && service.ServiceLookup?.LookupValue != null)
                .ToList();

            return configuredServices
                .Where(service => requested.Contains(service.ServiceLookup!.LookupValue, StringComparer.OrdinalIgnoreCase))
                .Sum(service => service.Price);
        }

        private static List<string> BuildRequestedServiceValues(bool examSimulation, bool recordedCourse, bool liveCourse)
        {
            var requested = new List<string>();
            if (examSimulation) requested.Add("EXAM_SIMULATION");
            if (recordedCourse) requested.Add("RECORDED_COURSE");
            if (liveCourse) requested.Add("LIVE_COURSE");
            return requested;
        }

        private async Task PopulateServicePricesAsync(StudentBasketDto item)
        {
            await PopulateServicePricesAsync(new List<StudentBasketDto> { item });
        }

        private async Task PopulateServicePricesAsync(List<StudentBasketDto> items)
        {
            if (items.Count == 0)
                return;

            var pricesByCourseId = new Dictionary<Guid, Dictionary<string, decimal>>(items.Count);
            foreach (var courseId in items.Select(x => x.CourseId).Distinct())
            {
                var servicePrices = (await _courseServiceRepository.GetByCourseIdAsync(courseId))
                    .Where(service => service.IsActive && !service.IsDeleted && service.ServiceLookup?.LookupValue != null)
                    .ToDictionary(service => service.ServiceLookup!.LookupValue, service => service.Price, StringComparer.OrdinalIgnoreCase);
                pricesByCourseId[courseId] = servicePrices;
            }

            foreach (var item in items)
            {
                if (!pricesByCourseId.TryGetValue(item.CourseId, out var servicePrices))
                    continue;

                item.ExamSimulationReservPrice = servicePrices.TryGetValue("EXAM_SIMULATION", out var examPrice) ? examPrice : null;
                item.RecordedCourseReservPrice = servicePrices.TryGetValue("RECORDED_COURSE", out var recordedPrice) ? recordedPrice : null;
                item.LiveCourseReservPrice = servicePrices.TryGetValue("LIVE_COURSE", out var livePrice) ? livePrice : null;
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
                AddedAt = entity.AddedAt
            };
        }
    }
}
