using LogicLib1.AppModels1.Server.Booking;
using System.Collections.Concurrent;

namespace BlazorApp1.Services;

public sealed class InMemoryBookingPaymentStore : IBookingPaymentStore
{
    private readonly ConcurrentDictionary<string, BookingPaymentRecord> _byBookingId = new();
    private readonly ConcurrentDictionary<string, string> _bookingIdByPaymentIntentId = new();

    public Task SaveAsync(BookingPaymentRecord record, CancellationToken ct = default)
    {
        _byBookingId[record.BookingId] = record;

        if (!string.IsNullOrWhiteSpace(record.PaymentIntentId))
            _bookingIdByPaymentIntentId[record.PaymentIntentId] = record.BookingId;

        return Task.CompletedTask;
    }

    public Task<BookingPaymentRecord?> GetByBookingIdAsync(string bookingId, CancellationToken ct = default)
    {
        _byBookingId.TryGetValue(bookingId, out var record);
        return Task.FromResult(record);
    }

    public Task<BookingPaymentRecord?> GetByPaymentIntentIdAsync(string paymentIntentId, CancellationToken ct = default)
    {
        if (_bookingIdByPaymentIntentId.TryGetValue(paymentIntentId, out var bookingId) &&
            _byBookingId.TryGetValue(bookingId, out var record))
        {
            return Task.FromResult<BookingPaymentRecord?>(record);
        }

        return Task.FromResult<BookingPaymentRecord?>(null);
    }

    public Task UpdateAsync(BookingPaymentRecord record, CancellationToken ct = default)
    {
        _byBookingId[record.BookingId] = record;

        if (!string.IsNullOrWhiteSpace(record.PaymentIntentId))
            _bookingIdByPaymentIntentId[record.PaymentIntentId] = record.BookingId;

        return Task.CompletedTask;
    }
}