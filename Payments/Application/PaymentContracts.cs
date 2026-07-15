using System.Text.Json.Serialization;
using HelpEmpowermentApi.Payments.Domain;

namespace HelpEmpowermentApi.Payments.Application;

public sealed record ServiceResult<T>(bool IsSuccess, T? Value, string? ErrorCode, string? ErrorMessage)
{
    public static ServiceResult<T> Success(T value) => new(true, value, null, null);
    public static ServiceResult<T> Failure(string code, string message) => new(false, default, code, message);
}
public sealed record CreateTelrPaymentRequest(Guid InvoiceId);
public sealed record CheckoutTelrRequest(string? CouponCode);
public sealed record CheckoutTelrResponse(Guid InvoiceId, string InvoiceNumber, decimal Amount, string Currency, CreateTelrPaymentResponse Payment);
public sealed record CreateTelrPaymentResponse(Guid PaymentId, string CartId, string TelrOrderReference, string PaymentUrl, PaymentStatus Status);
public sealed record TelrCreatePaymentCommand(Guid InvoiceId);
public sealed record PaymentStatusResponse(Guid PaymentId, Guid InvoiceId, string InvoiceNumber, PaymentStatus Status, bool IsPaid, decimal Amount, string Currency, string? TransactionReference, string? AuthCode, string? AuthMessage, DateTime? PaidAt);

public sealed class TelrCreateOrderRequest
{
    [JsonPropertyName("method")] public string Method { get; init; } = "create";
    [JsonPropertyName("store")] public required string Store { get; init; }
    [JsonPropertyName("authkey")] public required string AuthKey { get; init; }
    [JsonPropertyName("order")] public required TelrOrderInput Order { get; init; }
    [JsonPropertyName("return")] public required TelrReturnUrls Return { get; init; }
    [JsonPropertyName("panels")] public string? Panels { get; init; }
}
public sealed record TelrOrderInput([property: JsonPropertyName("cartid")] string CartId, [property: JsonPropertyName("test")] string Test, [property: JsonPropertyName("amount")] string Amount, [property: JsonPropertyName("currency")] string Currency, [property: JsonPropertyName("description")] string Description);
public sealed record TelrReturnUrls([property: JsonPropertyName("authorised")] string Authorised, [property: JsonPropertyName("declined")] string Declined, [property: JsonPropertyName("cancelled")] string Cancelled);
public sealed class TelrCreateOrderResponse { [JsonPropertyName("order")] public TelrCreatedOrder? Order { get; init; } [JsonPropertyName("error")] public TelrError? Error { get; init; } }
public sealed record TelrCreatedOrder([property: JsonPropertyName("ref")] string? Reference, [property: JsonPropertyName("url")] string? Url);
public sealed record TelrError([property: JsonPropertyName("message")] string? Message, [property: JsonPropertyName("note")] string? Note);
public sealed class TelrCheckOrderRequest { [JsonPropertyName("method")] public string Method { get; init; } = "check"; [JsonPropertyName("store")] public required string Store { get; init; } [JsonPropertyName("authkey")] public required string AuthKey { get; init; } [JsonPropertyName("order")] public required TelrOrderReference Order { get; init; } }
public sealed record TelrOrderReference([property: JsonPropertyName("ref")] string Reference);
public sealed class TelrCheckOrderResponse { [JsonPropertyName("invoice")] public TelrCheckedOrder? Invoice { get; init; } [JsonPropertyName("order")] public TelrCheckedOrder? Order { get; init; } [JsonPropertyName("error")] public TelrError? Error { get; init; } }
public sealed class TelrCheckedOrder { [JsonPropertyName("ref")] public string? Reference { get; init; } [JsonPropertyName("cartid")] public string? CartId { get; init; } [JsonPropertyName("amount")] public string? Amount { get; init; } [JsonPropertyName("currency")] public string? Currency { get; init; } [JsonPropertyName("status")] public TelrOrderStatus? Status { get; init; } [JsonPropertyName("transaction")] public TelrTransactionInfo? Transaction { get; init; } }
public sealed record TelrOrderStatus([property: JsonPropertyName("code")] int Code, [property: JsonPropertyName("text")] string? Text);
public sealed class TelrTransactionInfo { [JsonPropertyName("ref")] public string? Reference { get; init; } [JsonPropertyName("status")] public string? Status { get; init; } [JsonPropertyName("code")] public string? Code { get; init; } [JsonPropertyName("message")] public string? Message { get; init; } }
public sealed record TelrCreateResult(string OrderReference, string PaymentUrl, string RawRequest, string RawResponse);
public sealed record TelrCheckResult(bool IsAuthorised, bool IsOnHold, bool IsDeclined, string TelrStatus, decimal Amount, string Currency, string CartId, string OrderReference, string? TransactionReference, string? AuthCode, string? AuthMessage, string RawRequest, string RawResponse);

public sealed class TelrWebhookRequest
{
    public string? tran_store { get; set; } public string? tran_type { get; set; } public string? tran_class { get; set; } public string? tran_test { get; set; }
    public string? tran_ref { get; set; } public string? tran_prevref { get; set; } public string? tran_firstref { get; set; } public string? tran_order { get; set; }
    public string? tran_currency { get; set; } public string? tran_amount { get; set; } public string? tran_cartid { get; set; } public string? tran_desc { get; set; }
    public string? tran_status { get; set; } public string? tran_authcode { get; set; } public string? tran_authmessage { get; set; } public string? tran_check { get; set; }
    public string? card_last4 { get; set; } public string? card_code { get; set; } public string? card_payment { get; set; }
}

public interface ITelrPaymentService { Task<ServiceResult<TelrCreateResult>> CreatePaymentAsync(string cartId, decimal amount, string currency, string description, CancellationToken cancellationToken); Task<ServiceResult<TelrCheckResult>> CheckPaymentAsync(string orderReference, CancellationToken cancellationToken); }
public interface IPaymentTransactionService { Task<ServiceResult<CreateTelrPaymentResponse>> CreateAsync(Guid invoiceId, CancellationToken cancellationToken); Task<PaymentStatusResponse?> GetStatusAsync(Guid paymentId, CancellationToken cancellationToken); Task<PaymentTransaction?> FindAsync(string? orderReference, string? cartId, string? transactionReference, CancellationToken cancellationToken); Task ApplyCheckedStatusAsync(Guid paymentId, TelrCheckResult check, CancellationToken cancellationToken); }
public interface ITelrWebhookValidator { bool IsValid(IReadOnlyDictionary<string, string?> values); }
public interface IInvoicePaymentProcessor { Task<ServiceResult<bool>> ProcessAsync(Guid paymentId, TelrCheckResult result, CancellationToken cancellationToken); }
public interface IClock { DateTime UtcNow { get; } }
