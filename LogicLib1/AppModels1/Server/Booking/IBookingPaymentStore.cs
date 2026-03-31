using LogicLib1.AppModels1.Server.Booking;

namespace BlazorApp1.Services;

public interface IBookingPaymentStore
{
    Task SaveAsync(BookingPaymentRecord record, CancellationToken ct = default);
    Task<BookingPaymentRecord?> GetByBookingIdAsync(string bookingId, CancellationToken ct = default);
    Task<BookingPaymentRecord?> GetByPaymentIntentIdAsync(string paymentIntentId, CancellationToken ct = default);
    Task UpdateAsync(BookingPaymentRecord record, CancellationToken ct = default);
}