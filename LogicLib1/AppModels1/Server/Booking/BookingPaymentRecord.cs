using LogicLib1.AppModels1.Client;

namespace LogicLib1.AppModels1.Server.Booking;

public sealed class BookingPaymentRecord
{
    public string        BookingId       { get; set; } = "";
    public string        Category        { get; set; } = "";
    public string        PaymentIntentId { get; set; } = "";
    public string        PaymentStatus   { get; set; } = "";
    public bool          EmailSent       { get; set; }
    public DateTime      CreatedAt       { get; set; }
    public DateTime?     PaidAt { get; set; }
    public ClientRequest Request { get; set; } = new();
}