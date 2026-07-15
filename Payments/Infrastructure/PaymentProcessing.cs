using System.Data;
using System.Globalization;
using System.Text.Json;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Payments.Application;
using HelpEmpowermentApi.Payments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
                var payment = await db.PaymentTransactions.AsTracking().Include(x => x.Invoice).Include(x => x.Receipt).Include(x => x.JournalEntry).SingleOrDefaultAsync(x => x.Id == paymentId, ct);
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
                if (payment.Receipt is null) db.PaymentReceipts.Add(new() { Id = Guid.NewGuid(), PaymentTransactionId = payment.Id, ReceiptNumber = $"R-{payment.Id:N}", Amount = payment.Amount, Currency = payment.Currency, CreatedAt = now });
                if (payment.JournalEntry is null) db.PaymentJournalEntries.Add(new() { Id = Guid.NewGuid(), PaymentTransactionId = payment.Id, EntryNumber = $"J-{payment.Id:N}", Amount = payment.Amount, Currency = payment.Currency, CreatedAt = now });
                await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct); return ServiceResult<bool>.Success(true);
            });
        }
        catch (DbUpdateConcurrencyException ex) { logger.LogInformation(ex, "Concurrent completion for payment {PaymentId}", paymentId); return ServiceResult<bool>.Failure("CONCURRENT_PROCESSING", "Payment is already being processed."); }
    }
}
