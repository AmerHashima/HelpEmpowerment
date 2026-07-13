# Telr hosted payment integration

The API uses Telr's hosted payment page. It never receives card numbers or CVV values.

## Configuration

Set public/non-secret settings in `appsettings.Production.json`. Supply secrets through environment variables or a secret manager:

```powershell
$env:Telr__StoreId="12345"
$env:Telr__AuthKey="your-hosted-page-auth-key"
$env:Telr__WebhookSecret="your-webhook-secret"
```

`StoreId` is the numeric merchant store ID. Obtain `AuthKey` from Telr Merchant Administration > Hosted Payment Page > Integrations. Configure `IsTest=true` for the test system. Set the three return URLs to this API's public HTTPS `/authorised`, `/declined`, and `/cancelled` endpoints, and configure `/api/payments/telr/webhook` as Telr's form-urlencoded webhook URL. Never commit populated secrets.

Apply the schema with `dotnet ef database update`. For local callbacks, expose the HTTPS API with a trusted tunnel (for example `ngrok http https://localhost:5001`) and use its stable HTTPS hostname for every Telr callback URL.

Before production, use production HTTPS hostnames, set `IsTest=false`, restrict `SupportedCurrencies`, configure the server's live IP in Telr, and verify all callback paths. Inspect structured logs for the CartId, payment ID, Telr HTTP errors, status parsing errors, and reconciliation outcomes; secrets and signature/card-code fields are sanitized.

To manually reconcile, locate the payment by payment ID or CartId, take its stored Telr order reference, invoke Telr's Check Status API, and pass an authorised result through `IInvoicePaymentProcessor` (the background worker performs this automatically). Do not directly edit invoice paid fields.

Rotate secrets by adding the replacement in Telr, updating the application secret store, restarting instances, sending a signed test webhook, and only then revoking the old value. During a webhook-secret rotation deploy instances together because signatures can be validated with only the configured current secret.

## API

- `POST /api/payments/telr/create` and `GET /api/payments/telr/status/{paymentId}` require JWT authorization and are rate limited.
- Return endpoints never trust browser query values; they call Check Status.
- The webhook validates `tran_check`, then calls Check Status before completion.

The authoritative invoice amount and currency come from SQL Server. Seed or create an `Invoice` before initiating payment.
