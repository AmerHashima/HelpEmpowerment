using System.Globalization;
using System.Text.Json;
using HelpEmpowermentApi.Payments.Application;
using HelpEmpowermentApi.Payments.Domain;
using HelpEmpowermentApi.Payments.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using HelpEmpowermentApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Payments.Api;

[ApiController, Route("api/payments/telr")]
public sealed class TelrPaymentsController(IPaymentTransactionService payments, ITelrPaymentService telr, ITelrWebhookValidator webhookValidator, ApplicationDbContext db, IOptions<TelrOptions> options, ILogger<TelrPaymentsController> logger) : ControllerBase
{
    private readonly TelrOptions _options = options.Value;

    [HttpPost("checkout"), Authorize, EnableRateLimiting("payments-create")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutTelrRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var studentId))
            return ProblemResult(401, "INVALID_IDENTITY", "The authenticated student identifier is invalid.");

        var basket = await db.StudentBaskets.Include(x => x.Course)
            .Where(x => x.StudentId == studentId && !x.IsDeleted).ToListAsync(ct);
        if (basket.Count == 0) return ProblemResult(422, "BASKET_EMPTY", "The basket is empty.");

        decimal discountPercent = 0;
        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var promo = await db.Students.SingleOrDefaultAsync(x => x.PromoCode == request.CouponCode && !x.IsDeleted, ct);
            if (promo is null || promo.PromoToDateValid < DateTime.UtcNow) return ProblemResult(422, "INVALID_COUPON", "The coupon is invalid or expired.");
            discountPercent = Convert.ToDecimal(promo.PromoDiscount ?? 0, CultureInfo.InvariantCulture);
            if (discountPercent is < 0 or > 100) return ProblemResult(422, "INVALID_COUPON", "The coupon discount is invalid.");
        }

        var invoice = new Invoice { Id = Guid.NewGuid(), OwnerId = studentId, InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Convert.ToHexString(Guid.NewGuid().ToByteArray())[..6]}", Currency = _options.DefaultCurrency.ToUpperInvariant(), PromoCode = string.IsNullOrWhiteSpace(request.CouponCode) ? null : request.CouponCode.Trim(), CreatedAt = DateTime.UtcNow };
        foreach (var basketItem in basket)
        {
            if (basketItem.Quantity <= 0 || basketItem.Course is null) return ProblemResult(422, "INVALID_BASKET", "The basket contains an invalid item.");
            if (!decimal.TryParse(basketItem.Course.Price, NumberStyles.Number, CultureInfo.InvariantCulture, out var basePrice)) basePrice = 0;
            decimal servicesPrice = 0;
            if (basketItem.RecordedCourseReserv) servicesPrice += basketItem.Course.RecordedCourseReservPrice ?? 0;
            if (basketItem.ExamSimulationReserv) servicesPrice += basketItem.Course.ExamSimulationReservPrice ?? 0;
            if (basketItem.LiveCourseReserv) servicesPrice += basketItem.Course.LiveCourseReservPrice ?? 0;
            var unitPrice = basePrice + servicesPrice;
            var gross = unitPrice * basketItem.Quantity;
            var discount = decimal.Round(gross * discountPercent / 100m, 2, MidpointRounding.AwayFromZero);
            invoice.Items.Add(new InvoiceItem { Id = Guid.NewGuid(), BasketItemId = basketItem.Oid, CourseId = basketItem.CourseId, Description = basketItem.Course.CourseName ?? $"Course {basketItem.CourseId}", Quantity = basketItem.Quantity, UnitPrice = unitPrice, DiscountAmount = discount, LineTotal = gross - discount, ExamSimulationReserv = basketItem.ExamSimulationReserv, RecordedCourseReserv = basketItem.RecordedCourseReserv, LiveCourseReserv = basketItem.LiveCourseReserv });
        }
        invoice.TotalAmount = invoice.Items.Sum(x => x.LineTotal);
        if (invoice.TotalAmount <= 0) return ProblemResult(422, "INVALID_CHECKOUT_TOTAL", "The checkout total must be greater than zero.");

        db.Invoices.Add(invoice);
        await db.SaveChangesAsync(ct);
        var payment = await payments.CreateAsync(invoice.Id, ct);
        if (!payment.IsSuccess || payment.Value is null)
            return ProblemResult(payment.ErrorCode is "TELR_TIMEOUT" or "TELR_NETWORK_ERROR" ? 503 : 422, payment.ErrorCode!, payment.ErrorMessage!);
        return Ok(new CheckoutTelrResponse(invoice.Id, invoice.InvoiceNumber, invoice.TotalAmount, invoice.Currency, payment.Value));
    }

    [HttpPost("create"), Authorize, EnableRateLimiting("payments-create")]
    public async Task<IActionResult> Create([FromBody] CreateTelrPaymentRequest request, CancellationToken ct)
    {
        if (request.InvoiceId == Guid.Empty) return ProblemResult(400, "INVALID_REQUEST", "InvoiceId is required.");
        var invoiceOwner = await db.Invoices.Where(x => x.Id == request.InvoiceId).Select(x => x.OwnerId).SingleOrDefaultAsync(ct);
        if (invoiceOwner.HasValue && !IsOwner(invoiceOwner.Value)) return ProblemResult(403, "INVOICE_ACCESS_DENIED", "You do not have permission to pay this invoice.");
        var result = await payments.CreateAsync(request.InvoiceId, ct);
        if (result.IsSuccess) return Ok(result.Value);
        return result.ErrorCode switch { "INVOICE_NOT_FOUND" => ProblemResult(404, result.ErrorCode, result.ErrorMessage!), "INVOICE_ALREADY_PAID" => ProblemResult(409, result.ErrorCode, result.ErrorMessage!), "TELR_NETWORK_ERROR" or "TELR_TIMEOUT" => ProblemResult(503, result.ErrorCode, result.ErrorMessage!), _ => ProblemResult(422, result.ErrorCode!, result.ErrorMessage!) };
    }

    [HttpGet("status/{paymentId:guid}"), Authorize, EnableRateLimiting("payments-status")]
    public async Task<IActionResult> Status(Guid paymentId, CancellationToken ct)
    {
        var owner = await db.PaymentTransactions.Where(x => x.Id == paymentId).Select(x => x.Invoice.OwnerId).SingleOrDefaultAsync(ct);
        if (owner.HasValue && !IsOwner(owner.Value)) return ProblemResult(403, "PAYMENT_ACCESS_DENIED", "You do not have permission to view this payment.");
        var result = await payments.GetStatusAsync(paymentId, ct);
        return result is null ? ProblemResult(404, "PAYMENT_NOT_FOUND", "Payment was not found.") : Ok(result);
    }

    [HttpGet("authorised"), AllowAnonymous]
    public Task<IActionResult> Authorised([FromQuery(Name = "order.ref")] string? orderReference, [FromQuery] string? cartId, CancellationToken ct) => HandleReturn(orderReference, cartId, "success", true, ct);
    [HttpGet("declined"), AllowAnonymous]
    public Task<IActionResult> Declined([FromQuery(Name = "order.ref")] string? orderReference, [FromQuery] string? cartId, CancellationToken ct) => HandleReturn(orderReference, cartId, "declined", true, ct);
    [HttpGet("cancelled"), AllowAnonymous]
    public Task<IActionResult> Cancelled([FromQuery(Name = "order.ref")] string? orderReference, [FromQuery] string? cartId, CancellationToken ct) => HandleReturn(orderReference, cartId, "cancelled", false, ct);

    [HttpPost("webhook"), AllowAnonymous, Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        if (!Request.HasFormContentType) return ProblemResult(415, "INVALID_CONTENT_TYPE", "A form-urlencoded payload is required.");
        var form = await Request.ReadFormAsync(ct);
        var values = form.ToDictionary(x => x.Key, x => (string?)x.Value.ToString(), StringComparer.Ordinal);
        if (!webhookValidator.IsValid(values)) return ProblemResult(401, "INVALID_WEBHOOK_SIGNATURE", "Webhook signature is invalid.");
        values.TryGetValue("tran_cartid", out var cartId); values.TryGetValue("tran_ref", out var txRef); values.TryGetValue("tran_order", out var orderRef);
        var payment = await payments.FindAsync(orderRef, cartId, txRef, ct);
        if (payment is null) { logger.LogWarning("Valid Telr webhook received for unknown CartId {CartId}", cartId); return Ok(); }
        payment.WebhookPayload = JsonSerializer.Serialize(values.Where(x => x.Key is not "tran_check" and not "card_code").ToDictionary());
        if (!string.IsNullOrWhiteSpace(payment.TelrOrderReference))
        {
            var check = await telr.CheckPaymentAsync(payment.TelrOrderReference, ct);
            if (check.IsSuccess && check.Value is not null) await payments.ApplyCheckedStatusAsync(payment.Id, check.Value, ct);
        }
        return Ok();
    }

    private async Task<IActionResult> HandleReturn(string? orderReference, string? cartId, string resultStatus, bool checkStatus, CancellationToken ct)
    {
        var payment = await payments.FindAsync(orderReference, cartId, null, ct);
        if (payment is null) return Redirect(ResultUrl(null, "unknown"));
        if (checkStatus && !string.IsNullOrWhiteSpace(payment.TelrOrderReference))
        {
            var check = await telr.CheckPaymentAsync(payment.TelrOrderReference, ct);
            if (check.IsSuccess && check.Value is not null) { await payments.ApplyCheckedStatusAsync(payment.Id, check.Value, ct); resultStatus = check.Value.IsAuthorised ? "success" : check.Value.IsDeclined ? "declined" : "pending"; }
        }
        else if (payment.Status != PaymentStatus.Authorised) { payment.Status = PaymentStatus.Cancelled; payment.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(ct); }
        return Redirect(ResultUrl(payment.Id, resultStatus));
    }
    private string ResultUrl(Guid? id, string status) => $"{_options.FrontendResultUrl.GetLeftPart(UriPartial.Path)}?paymentId={Uri.EscapeDataString(id?.ToString() ?? "")}&status={Uri.EscapeDataString(status)}";
    private bool IsOwner(Guid ownerId) => User.IsInRole("ADMIN") || Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var currentId) && currentId == ownerId;
    private ObjectResult ProblemResult(int status, string code, string detail) { var p = new ProblemDetails { Type = $"https://httpstatuses.com/{status}", Title = "Payment request failed", Status = status, Detail = detail }; p.Extensions["traceId"] = HttpContext.TraceIdentifier; p.Extensions["errorCode"] = code; return StatusCode(status, p); }
}
