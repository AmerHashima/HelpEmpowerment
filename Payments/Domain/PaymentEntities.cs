using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HelpEmpowermentApi.Payments.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus { Created, Pending, Redirected, Authorised, OnHold, Declined, Cancelled, Failed, Expired, Refunded }

public sealed class Invoice
{
    public Guid Id { get; set; }
    public Guid? OwnerId { get; set; }
    [MaxLength(50)] public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    [MaxLength(3)] public string Currency { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    [MaxLength(50)] public string? PaymentMethod { get; set; }
    [MaxLength(50)] public string? PromoCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = [];
    public ICollection<InvoiceItem> Items { get; set; } = [];
}

public sealed class InvoiceItem
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public Guid CourseId { get; set; }
    [MaxLength(200)] public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LineTotal { get; set; }
    public bool ExamSimulationReserv { get; set; }
    public bool RecordedCourseReserv { get; set; }
    public bool LiveCourseReserv { get; set; }
}

public sealed class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    [MaxLength(50)] public string Provider { get; set; } = "Telr";
    [MaxLength(100)] public string CartId { get; set; } = string.Empty;
    [MaxLength(100)] public string? TelrOrderReference { get; set; }
    [MaxLength(100)] public string? TelrTransactionReference { get; set; }
    public decimal Amount { get; set; }
    [MaxLength(3)] public string Currency { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    [MaxLength(100)] public string? AuthCode { get; set; }
    [MaxLength(500)] public string? AuthMessage { get; set; }
    public string? PaymentUrl { get; set; }
    public string? CreateRequest { get; set; }
    public string? CreateResponse { get; set; }
    public string? CheckRequest { get; set; }
    public string? CheckResponse { get; set; }
    public string? WebhookPayload { get; set; }
    [MaxLength(1000)] public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public int RetryCount { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; } = [];
    public PaymentReceipt? Receipt { get; set; }
    public PaymentJournalEntry? JournalEntry { get; set; }
}

public sealed class PaymentReceipt
{
    public Guid Id { get; set; }
    public Guid PaymentTransactionId { get; set; }
    public PaymentTransaction PaymentTransaction { get; set; } = null!;
    [MaxLength(50)] public string ReceiptNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    [MaxLength(3)] public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class PaymentJournalEntry
{
    public Guid Id { get; set; }
    public Guid PaymentTransactionId { get; set; }
    public PaymentTransaction PaymentTransaction { get; set; } = null!;
    [MaxLength(50)] public string EntryNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    [MaxLength(3)] public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
