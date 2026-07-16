using System.Data;
using System.Globalization;
using System.Text.Json;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Payments.Application;
using HelpEmpowermentApi.Payments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models = HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Payments.Infrastructure;

public sealed class PaymentTransactionService(ApplicationDbContext db, ITelrPaymentService telr, IInvoicePaymentProcessor processor, IClock clock, IOptions<TelrOptions> options, ILogger<PaymentTransactionService> logger) : IPaymentTransactionService
{
    private readonly TelrOptions _options = options.Value;
    public async Task<ServiceResult<CreateTelrPaymentResponse>> CreateAsync(Guid invoiceId, CancellationToken ct)
    {
        var invoice = await db.Invoices.SingleOrDefaultAsync(x => x.Id == invoiceId, ct);
        if (invoice is null) return ServiceResult<CreateTelrPaymentResponse>.Failure("INVOICE_NOT_FOUND", "Invoice was not found.");
        if (invoice.IsPaid) return ServiceResult<CreateTelrPaymentResponse>.Failure("INVOICE_ALREADY_PAID", "Invoice is already paid.");
        if (invoice.TotalAmount <= 0) return ServiceResult<CreateTelrPaymentResponse>.Failure("INVALID_INVOICE_TOTAL", "Invoice total must be greater than zero.");
        var currency = invoice.Currency.Trim().ToUpperInvariant();
        if (!_options.SupportedCurrencies.Contains(currency, StringComparer.OrdinalIgnoreCase)) return ServiceResult<CreateTelrPaymentResponse>.Failure("UNSUPPORTED_CURRENCY", "Invoice currency is not supported.");
        var paymentId = Guid.NewGuid();
        var cleanInvoice = new string(invoice.InvoiceNumber.Where(char.IsLetterOrDigit).Take(20).ToArray());
        var cartId = $"{cleanInvoice}-{paymentId:N}-{Convert.ToHexString(Guid.NewGuid().ToByteArray())[..8]}";
        var payment = new PaymentTransaction { Id = paymentId, InvoiceId = invoice.Id, Provider = "Telr", CartId = cartId, Amount = invoice.TotalAmount, Currency = currency, Status = PaymentStatus.Created, CreatedAt = clock.UtcNow };
        db.PaymentTransactions.Add(payment);
        await db.SaveChangesAsync(ct);
        var result = await telr.CreatePaymentAsync(cartId, payment.Amount, currency, $"Invoice {invoice.InvoiceNumber}", ct);
        payment.UpdatedAt = clock.UtcNow;
        if (!result.IsSuccess || result.Value is null)
        {
            payment.Status = result.ErrorCode is "TELR_TIMEOUT" or "TELR_NETWORK_ERROR" ? PaymentStatus.Pending : PaymentStatus.Failed;
            payment.FailureReason = result.ErrorMessage;
            await db.SaveChangesAsync(ct);
            return ServiceResult<CreateTelrPaymentResponse>.Failure(result.ErrorCode!, result.ErrorMessage!);
        }
        payment.TelrOrderReference = result.Value.OrderReference; payment.PaymentUrl = result.Value.PaymentUrl; payment.CreateRequest = result.Value.RawRequest; payment.CreateResponse = result.Value.RawResponse; payment.Status = PaymentStatus.Redirected;
        await db.SaveChangesAsync(ct);
        return ServiceResult<CreateTelrPaymentResponse>.Success(new(payment.Id, payment.CartId, result.Value.OrderReference, result.Value.PaymentUrl, payment.Status));
    }

    public async Task<PaymentStatusResponse?> GetStatusAsync(Guid paymentId, CancellationToken ct)
    {
        var p = await db.PaymentTransactions.Include(x => x.Invoice).SingleOrDefaultAsync(x => x.Id == paymentId, ct);
        return p is null ? null : new(p.Id, p.InvoiceId, p.Invoice.InvoiceNumber, p.Status, p.Invoice.IsPaid, p.Amount, p.Currency, p.TelrTransactionReference, p.AuthCode, p.AuthMessage, p.PaidAt);
    }

    public Task<PaymentTransaction?> FindAsync(string? orderReference, string? cartId, string? transactionReference, CancellationToken ct) =>
        db.PaymentTransactions.AsTracking().FirstOrDefaultAsync(p => (orderReference != null && p.TelrOrderReference == orderReference) || (cartId != null && p.CartId == cartId) || (transactionReference != null && p.TelrTransactionReference == transactionReference), ct);

    public async Task ApplyCheckedStatusAsync(Guid paymentId, TelrCheckResult check, CancellationToken ct)
    {
        if (check.IsAuthorised) { await processor.ProcessAsync(paymentId, check, ct); return; }
        var p = await db.PaymentTransactions.AsTracking().SingleOrDefaultAsync(x => x.Id == paymentId, ct);
        if (p is null || p.Status == PaymentStatus.Authorised) return;
        p.CheckRequest = check.RawRequest; p.CheckResponse = check.RawResponse; p.UpdatedAt = clock.UtcNow;
        p.Status = check.IsOnHold ? PaymentStatus.OnHold : check.IsDeclined ? PaymentStatus.Declined : PaymentStatus.Pending;
        await db.SaveChangesAsync(ct);
    }
}

public sealed class InvoicePaymentProcessor(ApplicationDbContext db, IClock clock, ILogger<InvoicePaymentProcessor> logger) : IInvoicePaymentProcessor
{
    public async Task<ServiceResult<bool>> ProcessAsync(Guid paymentId, TelrCheckResult result, CancellationToken ct)
    {
        try
        {
            var strategy = db.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
                var payment = await db.PaymentTransactions.AsTracking()
                    .Include(x => x.Invoice).ThenInclude(invoice => invoice.Items)
                    .Include(x => x.Receipt)
                    .Include(x => x.JournalEntry)
                    .SingleOrDefaultAsync(x => x.Id == paymentId, ct);
                if (payment is null) return ServiceResult<bool>.Failure("PAYMENT_NOT_FOUND", "Payment was not found.");
                if (payment.Status == PaymentStatus.Authorised && payment.Invoice.IsPaid) { await transaction.CommitAsync(ct); return ServiceResult<bool>.Success(false); }
                if (!result.IsAuthorised) return ServiceResult<bool>.Failure("NOT_AUTHORISED", "Telr has not authorised this transaction.");
                if (payment.Amount != result.Amount) return ServiceResult<bool>.Failure("AMOUNT_MISMATCH", "Confirmed amount does not match the payment.");
                if (!string.Equals(payment.Currency, result.Currency, StringComparison.OrdinalIgnoreCase)) return ServiceResult<bool>.Failure("CURRENCY_MISMATCH", "Confirmed currency does not match the payment.");
                // Current Telr check responses omit cartid. Validate it when the gateway supplies it.
                if (!string.IsNullOrWhiteSpace(result.CartId) && !string.Equals(payment.CartId, result.CartId, StringComparison.Ordinal)) return ServiceResult<bool>.Failure("CART_ID_MISMATCH", "Confirmed CartId does not match the payment.");
                if (!string.Equals(payment.TelrOrderReference, result.OrderReference, StringComparison.Ordinal)) return ServiceResult<bool>.Failure("ORDER_REFERENCE_MISMATCH", "Confirmed order reference does not match the payment.");
                if (string.IsNullOrWhiteSpace(result.TransactionReference)) return ServiceResult<bool>.Failure("TRANSACTION_REFERENCE_MISSING", "Telr did not return a transaction reference.");
                if (payment.TelrTransactionReference is not null && payment.TelrTransactionReference != result.TransactionReference) return ServiceResult<bool>.Failure("TRANSACTION_REFERENCE_MISMATCH", "Confirmed transaction reference does not match the payment.");
                var now = clock.UtcNow;
                payment.Status = PaymentStatus.Authorised; payment.TelrTransactionReference = result.TransactionReference; payment.AuthCode = result.AuthCode; payment.AuthMessage = result.AuthMessage; payment.CheckRequest = result.RawRequest; payment.CheckResponse = result.RawResponse; payment.PaidAt = now; payment.UpdatedAt = now;
                payment.Invoice.IsPaid = true; payment.Invoice.PaidAt = now; payment.Invoice.PaymentMethod = "Telr";
                if (!payment.Invoice.OwnerId.HasValue)
                    return ServiceResult<bool>.Failure("INVOICE_OWNER_MISSING", "The paid invoice has no student owner.");

                var studentId = payment.Invoice.OwnerId.Value;
                var paidStatusId = Guid.Parse("88888888-8888-8888-8888-888888888802");
                var activeStatusId = Guid.Parse("99999999-9999-9999-9999-999999999901");
                var invoiceItemIds = payment.Invoice.Items.Select(item => item.Id).ToList();
                var fulfilledItemIds = await db.StudentCourses
                    .Where(course => course.InvoiceItemId.HasValue && invoiceItemIds.Contains(course.InvoiceItemId.Value))
                    .Select(course => course.InvoiceItemId!.Value)
                    .ToListAsync(ct);

                foreach (var item in payment.Invoice.Items.Where(item => !fulfilledItemIds.Contains(item.Id)))
                {
                    var enrollment = await db.StudentCourses.AsTracking()
                        .Include(course => course.Reservations)
                        .SingleOrDefaultAsync(course => course.StudentId == studentId
                            && course.CourseId == item.CourseId
                            && !course.IsDeleted, ct);

                    if (enrollment is null)
                    {
                        enrollment = new Models.StudentCourse
                        {
                            Oid = Guid.NewGuid(),
                            InvoiceItemId = item.Id,
                            StudentId = studentId,
                            CourseId = item.CourseId,
                            PaymentStatusLookupId = paidStatusId,
                            EnrollmentStatusLookupId = activeStatusId,
                            Price = item.UnitPrice * item.Quantity,
                            DiscountAmount = item.DiscountAmount,
                            PaidAmount = item.LineTotal,
                            PaymentMethod = "Telr",
                            TransactionId = result.TransactionReference,
                            PaymentDate = now,
                            EnrollmentDate = now,
                            ExamSimulationReserv = item.ExamSimulationReserv,
                            RecordedCourseReserv = item.RecordedCourseReserv,
                            LiveCourseReserv = item.LiveCourseReserv,
                            CreatedBy = studentId,
                            CreatedAt = now
                        };
                        db.StudentCourses.Add(enrollment);
                    }

                    var requestedServiceValues = new List<string>();
                    if (item.ExamSimulationReserv) requestedServiceValues.Add("EXAM_SIMULATION");
                    if (item.RecordedCourseReserv) requestedServiceValues.Add("RECORDED_COURSE");
                    if (item.LiveCourseReserv) requestedServiceValues.Add("LIVE_COURSE");

                    if (requestedServiceValues.Count > 0)
                    {
                        var courseServices = await db.CourseServices.AsNoTracking()
                            .Where(service => service.CourseId == item.CourseId
                                && service.IsActive
                                && !service.IsDeleted
                                && requestedServiceValues.Contains(service.ServiceLookup.LookupValue))
                            .Select(service => new { service.Oid, service.Price })
                            .ToListAsync(ct);
                        if (courseServices.Count != requestedServiceValues.Distinct(StringComparer.OrdinalIgnoreCase).Count())
                            return ServiceResult<bool>.Failure(
                                "COURSE_SERVICE_CONFIGURATION_MISSING",
                                $"One or more paid services are not configured for course {item.CourseId}.");
                        var existingServiceIds = enrollment.Reservations
                            .Where(reservation => !reservation.IsDeleted)
                            .Select(reservation => reservation.CourseServiceId)
                            .ToHashSet();

                        foreach (var service in courseServices.Where(service => !existingServiceIds.Contains(service.Oid)))
                        {
                            db.StudentCourseReservations.Add(new Models.StudentCourseReservation
                            {
                                Oid = Guid.NewGuid(),
                                StudentCourseId = enrollment.Oid,
                                CourseServiceId = service.Oid,
                                ServicePrice = service.Price,
                                IsReserved = false,
                                CreatedBy = studentId,
                                CreatedAt = now
                            });
                        }
                    }
                }

                var basketItemIds = payment.Invoice.Items
                    .Where(item => item.BasketItemId.HasValue)
                    .Select(item => item.BasketItemId!.Value)
                    .ToList();
                if (basketItemIds.Count > 0)
                {
                    var purchasedBasketItems = await db.StudentBaskets.AsTracking()
                        .Where(item => item.StudentId == studentId && basketItemIds.Contains(item.Oid))
                        .ToListAsync(ct);
                    db.StudentBaskets.RemoveRange(purchasedBasketItems);
                }
                if (payment.Receipt is null) db.PaymentReceipts.Add(new() { Id = Guid.NewGuid(), PaymentTransactionId = payment.Id, ReceiptNumber = $"R-{payment.Id:N}", Amount = payment.Amount, Currency = payment.Currency, CreatedAt = now });
                if (payment.JournalEntry is null) db.PaymentJournalEntries.Add(new() { Id = Guid.NewGuid(), PaymentTransactionId = payment.Id, EntryNumber = $"J-{payment.Id:N}", Amount = payment.Amount, Currency = payment.Currency, CreatedAt = now });
                await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct); return ServiceResult<bool>.Success(true);
            });
        }
        catch (DbUpdateConcurrencyException ex) { logger.LogInformation(ex, "Concurrent completion for payment {PaymentId}", paymentId); return ServiceResult<bool>.Failure("CONCURRENT_PROCESSING", "Payment is already being processed."); }
    }
}
