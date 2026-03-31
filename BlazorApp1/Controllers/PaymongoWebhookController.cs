using BlazorApp1.Services;
using LogicLib1.AppEmailer1;
using LogicLib1.AppPayment1;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("api/paymongo/webhook")]
public sealed class PaymongoWebhookController : ControllerBase
{
    private readonly IBookingPaymentStore _store;
    private readonly IAppEmailer _emailer;
    private readonly BookingNotificationService _notifications;

    public PaymongoWebhookController(
        IBookingPaymentStore store,
        IAppEmailer emailer,
        BookingNotificationService notifications)
    {
        _store = store;
        _emailer = emailer;
        _notifications = notifications;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PaymongoWebhookEvent payload, CancellationToken ct)
    {
        var eventType = payload.Data.Attributes.Type;
        var paymentIntentId = payload.Data.Attributes.Data.Attributes.PaymentIntentId;

        if (string.IsNullOrWhiteSpace(eventType) || string.IsNullOrWhiteSpace(paymentIntentId))
            return Ok(new { message = "ignored" });

        var record = await _store.GetByPaymentIntentIdAsync(paymentIntentId, ct);
        if (record is null)
            return Ok(new { message = "record not found" });

        if (string.Equals(eventType, "payment.paid", StringComparison.OrdinalIgnoreCase))
        {
            record.PaymentStatus = "Paid";
            record.PaidAt = payload.Data.Attributes.Data.Attributes.PaidAtUnix.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(payload.Data.Attributes.Data.Attributes.PaidAtUnix.Value).UtcDateTime
                : DateTime.UtcNow;

            if (!record.EmailSent)
            {
                await _emailer.SendEmailAsync(record.Request);
                record.EmailSent = true;
            }

            await _store.UpdateAsync(record, ct);
            await _notifications.PublishAsync(record.BookingId, "Paid");

            return Ok(new { message = "payment recorded" });
        }

        if (string.Equals(eventType, "payment.failed", StringComparison.OrdinalIgnoreCase))
        {
            record.PaymentStatus = "Failed";
            await _store.UpdateAsync(record, ct);
            await _notifications.PublishAsync(record.BookingId, "Failed");

            return Ok(new { message = "failure recorded" });
        }

        if (string.Equals(eventType, "qrph.expired", StringComparison.OrdinalIgnoreCase))
        {
            record.PaymentStatus = "Expired";
            await _store.UpdateAsync(record, ct);
            await _notifications.PublishAsync(record.BookingId, "Expired");

            return Ok(new { message = "expiry recorded" });
        }

        return Ok(new { message = "event ignored" });
    }
}