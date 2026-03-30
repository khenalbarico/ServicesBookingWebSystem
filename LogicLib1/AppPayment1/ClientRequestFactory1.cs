using LogicLib1.AppModels1.Client;
using ToolsLib1.Paymongo1;

namespace LogicLib1.AppPayment1;

public static class ClientRequestFactory1
{
    public static PaymongoCheckoutReq BuildCheckoutRequest(this ClientRequest payload)
    {
        var client    = payload.ClientInformation;

        var fullName  = $"{client.FirstName} {client.LastName}".Trim();

        var lineItems = payload.ClientServices
            .Select(s => new PaymongoCheckoutLineItem
            {
                Name             = $"{s.ServiceName} - {s.ServiceDetails}",
                Description      = $"{s.ServiceName} on {s.ServiceDate:yyyy-MM-dd HH:mm}",
                Quantity         = 1,
                AmountInCentavos = s.ServiceCost * 100 
            })
            .ToList();

        return new PaymongoCheckoutReq
        {
            CustomerName    = fullName,
            CustomerEmail   = client.Email,
            CustomerPhone   = client.ContactNumber,
            AppointmentDate = client.BookingDate.ToString("yyyy-MM-dd"),
            AppointmentTime = client.BookingDate.ToString("HH:mm"),
            SuccessUrl      = "https://example.com/paymongo/success",
            CancelUrl       = "https://example.com/paymongo/cancel",
            Description     = $"Booking #{client.ClientBookingId}",
            LineItems       = lineItems
        };
    }
}
