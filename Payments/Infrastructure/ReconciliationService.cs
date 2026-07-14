using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.Payments.Application;
using HelpEmpowermentApi.Payments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HelpEmpowermentApi.Payments.Infrastructure;

public sealed class PaymentReconciliationService(IServiceScopeFactory scopes, IOptions<PaymentReconciliationOptions> options, ILogger<PaymentReconciliationService> logger) : BackgroundService
{
    private readonly PaymentReconciliationOptions _options = options.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(Math.Max(1, _options.IntervalMinutes)));
        do
        {
            try { await ReconcileAsync(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex) { logger.LogError(ex, "Payment reconciliation cycle failed; it will be retried on the next interval"); }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
    private async Task ReconcileAsync(CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var telr = scope.ServiceProvider.GetRequiredService<ITelrPaymentService>();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentTransactionService>();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();
        var now = clock.UtcNow;
        // READCOMMITTEDLOCK makes READPAST valid when READ_COMMITTED_SNAPSHOT is enabled.
        // UPDLOCK/READPAST prevents multiple application instances from selecting the same rows.
        var candidates = await db.PaymentTransactions.FromSqlInterpolated($@"SELECT TOP ({_options.BatchSize}) * FROM PaymentTransactions WITH (UPDLOCK, READPAST, ROWLOCK, READCOMMITTEDLOCK) WHERE Status IN ('Created','Pending','Redirected','OnHold') AND CreatedAt <= {now.AddMinutes(-_options.GracePeriodMinutes)} AND RetryCount < {_options.MaxRetryCount} ORDER BY CreatedAt").ToListAsync(ct);
        foreach (var payment in candidates)
        {
            try
            {
                if (payment.CreatedAt <= now.AddHours(-_options.ExpirationHours)) { payment.Status = PaymentStatus.Expired; payment.UpdatedAt = now; continue; }
                if (string.IsNullOrWhiteSpace(payment.TelrOrderReference)) { payment.RetryCount++; payment.UpdatedAt = now; continue; }
                var checkedResult = await telr.CheckPaymentAsync(payment.TelrOrderReference, ct);
                if (checkedResult.IsSuccess && checkedResult.Value is not null) await service.ApplyCheckedStatusAsync(payment.Id, checkedResult.Value, ct); else payment.RetryCount++;
            }
            catch (Exception ex) { payment.RetryCount++; logger.LogError(ex, "Reconciliation failed for {PaymentId}", payment.Id); }
        }
        await db.SaveChangesAsync(ct);
        if (candidates.Count > 0) logger.LogInformation("Reconciled {PaymentCount} Telr payments", candidates.Count);
    }
}
