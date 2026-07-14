namespace HelpEmpowermentApi.Payments.Infrastructure;

public sealed class TelrOptions
{
    public const string SectionName = "Telr";
    public bool Enabled { get; set; }
    public string StoreId { get; set; } = string.Empty;
    public string AuthKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public Uri CreateOrderUrl { get; set; } = new("https://secure.telr.com/gateway/order.json");
    public Uri CheckOrderUrl { get; set; } = new("https://secure.telr.com/gateway/order.json");
    public Uri AuthorisedReturnUrl { get; set; } = null!;
    public Uri DeclinedReturnUrl { get; set; } = null!;
    public Uri CancelledReturnUrl { get; set; } = null!;
    public Uri FrontendResultUrl { get; set; } = null!;
    public bool IsTest { get; set; }
    public string DefaultCurrency { get; set; } = "AED";
    public string EnabledPanels { get; set; } = "card";
    public string[] SupportedCurrencies { get; set; } = ["AED"];
}
public sealed class PaymentReconciliationOptions { public int BatchSize { get; set; } = 50; public int IntervalMinutes { get; set; } = 5; public int GracePeriodMinutes { get; set; } = 3; public int ExpirationHours { get; set; } = 24; public int MaxRetryCount { get; set; } = 10; }
public sealed class SystemClock : HelpEmpowermentApi.Payments.Application.IClock { public DateTime UtcNow => DateTime.UtcNow; }
