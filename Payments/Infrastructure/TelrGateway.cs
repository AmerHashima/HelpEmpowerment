using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HelpEmpowermentApi.Payments.Application;
using Microsoft.Extensions.Options;

namespace HelpEmpowermentApi.Payments.Infrastructure;

public sealed class TelrPaymentService(HttpClient client, IOptions<TelrOptions> options, IHttpContextAccessor httpContextAccessor, ILogger<TelrPaymentService> logger) : ITelrPaymentService
{
    private readonly TelrOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<ServiceResult<TelrCreateResult>> CreatePaymentAsync(string cartId, decimal amount, string currency, string description, CancellationToken ct)
    {
        var httpRequest = httpContextAccessor.HttpContext?.Request;
        if (httpRequest is null || !httpRequest.Host.HasValue)
            return ServiceResult<TelrCreateResult>.Failure("PUBLIC_HOST_REQUIRED", "The deployed public request host could not be determined.");

        // Forwarded-header middleware supplies the public scheme and host when TLS is
        // terminated by a reverse proxy. Do not force HTTPS for a host that only serves HTTP.
        var apiBaseUrl = $"{httpRequest.Scheme}://{httpRequest.Host.ToUriComponent()}{httpRequest.PathBase}".TrimEnd('/');
        var encodedCartId = Uri.EscapeDataString(cartId);
        var returnUrls = new TelrReturnUrls(
            $"{apiBaseUrl}/api/payments/telr/authorised?cartId={encodedCartId}",
            $"{apiBaseUrl}/api/payments/telr/declined?cartId={encodedCartId}",
            $"{apiBaseUrl}/api/payments/telr/cancelled?cartId={encodedCartId}");
        var request = new TelrCreateOrderRequest { Store = _options.StoreId, AuthKey = _options.AuthKey, Order = new(cartId, _options.IsTest ? "1" : "0", amount.ToString("0.00", CultureInfo.InvariantCulture), currency, description), Return = returnUrls, Panels = _options.EnabledPanels };
        var safeRequest = JsonSerializer.Serialize(WithAuthKey("***"), JsonOptions);
        try
        {
            logger.LogInformation("Creating Telr order for CartId {CartId}, Amount {Amount}, Currency {Currency}", cartId, amount, currency);
            using var response = await client.PostAsJsonAsync(_options.CreateOrderUrl, request, JsonOptions, ct);
            var raw = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode) return ServiceResult<TelrCreateResult>.Failure("TELR_HTTP_ERROR", $"Telr returned HTTP {(int)response.StatusCode}.");
            TelrCreateOrderResponse? parsed;
            try { parsed = JsonSerializer.Deserialize<TelrCreateOrderResponse>(raw, JsonOptions); }
            catch (JsonException) { return ServiceResult<TelrCreateResult>.Failure("TELR_INVALID_RESPONSE", "Telr returned malformed JSON."); }
            if (parsed?.Error is not null) return ServiceResult<TelrCreateResult>.Failure("TELR_BUSINESS_ERROR", parsed.Error.Message ?? parsed.Error.Note ?? "Telr rejected the order.");
            if (string.IsNullOrWhiteSpace(parsed?.Order?.Reference) || !Uri.TryCreate(parsed.Order.Url, UriKind.Absolute, out var paymentUri) || paymentUri.Scheme != Uri.UriSchemeHttps)
                return ServiceResult<TelrCreateResult>.Failure("TELR_INVALID_RESPONSE", "Telr response did not contain a valid order reference and HTTPS URL.");
            return ServiceResult<TelrCreateResult>.Success(new(parsed.Order.Reference, paymentUri.ToString(), safeRequest, Sanitize(raw)));
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return ServiceResult<TelrCreateResult>.Failure("TELR_TIMEOUT", "Telr did not respond in time; this payment will be reconciled."); }
        catch (HttpRequestException ex) { logger.LogWarning(ex, "Network error creating Telr order for {CartId}", cartId); return ServiceResult<TelrCreateResult>.Failure("TELR_NETWORK_ERROR", "Telr could not be reached; this payment will be reconciled."); }

        TelrCreateOrderRequest WithAuthKey(string key) => new() { Store = request.Store, AuthKey = key, Order = request.Order, Return = request.Return, Panels = request.Panels };
    }

    public async Task<ServiceResult<TelrCheckResult>> CheckPaymentAsync(string orderReference, CancellationToken ct)
    {
        var request = new TelrCheckOrderRequest { Store = _options.StoreId, AuthKey = _options.AuthKey, Order = new(orderReference) };
        var safeRequest = JsonSerializer.Serialize(new TelrCheckOrderRequest { Store = request.Store, AuthKey = "***", Order = request.Order }, JsonOptions);
        try
        {
            using var response = await client.PostAsJsonAsync(_options.CheckOrderUrl, request, JsonOptions, ct);
            var raw = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode) return ServiceResult<TelrCheckResult>.Failure("TELR_HTTP_ERROR", $"Telr returned HTTP {(int)response.StatusCode}.");
            TelrCheckOrderResponse? parsed;
            try { parsed = JsonSerializer.Deserialize<TelrCheckOrderResponse>(raw, JsonOptions); }
            catch (JsonException) { return ServiceResult<TelrCheckResult>.Failure("TELR_INVALID_RESPONSE", "Telr returned malformed JSON."); }
            // Current Telr check responses use "invoice"; retain "order" support for
            // older/alternate gateway responses.
            var o = parsed?.Invoice ?? parsed?.Order;
            if (parsed?.Error is not null) return ServiceResult<TelrCheckResult>.Failure("TELR_BUSINESS_ERROR", parsed.Error.Message ?? "Unable to check order.");
            if (o is null || string.IsNullOrWhiteSpace(o.Currency) || !decimal.TryParse(o.Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
                return ServiceResult<TelrCheckResult>.Failure("TELR_INVALID_RESPONSE", "Required check-status fields were missing or invalid.");
            var txStatus = o.Transaction?.Status?.Trim().ToUpperInvariant() ?? string.Empty;
            var orderText = o.Status?.Text?.Trim() ?? string.Empty;
            var authorised = txStatus == "A" && string.Equals(orderText, "Paid", StringComparison.OrdinalIgnoreCase);
            var onHold = txStatus == "H" || orderText.Contains("hold", StringComparison.OrdinalIgnoreCase);
            var declined = txStatus == "D" || orderText.Contains("declin", StringComparison.OrdinalIgnoreCase);
            return ServiceResult<TelrCheckResult>.Success(new(authorised, onHold, declined, txStatus.Length > 0 ? txStatus : orderText, amount, o.Currency.ToUpperInvariant(), o.CartId ?? string.Empty, orderReference, o.Transaction?.Reference, o.Transaction?.Code, o.Transaction?.Message, safeRequest, Sanitize(raw)));
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return ServiceResult<TelrCheckResult>.Failure("TELR_TIMEOUT", "Telr status check timed out."); }
        catch (HttpRequestException ex) { logger.LogWarning(ex, "Network error checking Telr order {OrderReference}", orderReference); return ServiceResult<TelrCheckResult>.Failure("TELR_NETWORK_ERROR", "Telr could not be reached."); }
    }

    private string Sanitize(string value) => value.Replace(_options.AuthKey, "***", StringComparison.Ordinal).Replace(_options.WebhookSecret, "***", StringComparison.Ordinal);
}

public sealed class TelrWebhookValidator(IOptions<TelrOptions> options) : ITelrWebhookValidator
{
    public static readonly string[] OrderedFields = ["tran_store", "tran_type", "tran_class", "tran_test", "tran_ref", "tran_prevref", "tran_firstref", "tran_currency", "tran_amount", "tran_cartid", "tran_desc", "tran_status", "tran_authcode", "tran_authmessage"];
    private readonly string _secret = options.Value.WebhookSecret;
    public bool IsValid(IReadOnlyDictionary<string, string?> values)
    {
        if (!values.TryGetValue("tran_check", out var supplied) || string.IsNullOrWhiteSpace(supplied) || string.IsNullOrEmpty(_secret)) return false;
        var verification = string.Join(':', new[] { _secret }.Concat(OrderedFields.Select(k => values.TryGetValue(k, out var v) ? v ?? "" : "")));
        var expected = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(verification))).ToLowerInvariant();
        if (supplied.Length != expected.Length) return false;
        return CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(expected), Encoding.ASCII.GetBytes(supplied.ToLowerInvariant()));
    }
}
