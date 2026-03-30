using System.Text;
using System.Text.Json;

namespace ToolsLib1.Paymongo1;

public sealed class Paymongo2(IPaymongoCfg _cfg) : IToolPaymongo
{
    private readonly HttpClient httpClient = _cfg.CreatePaymongoClient();

    public async Task<PaymongoCheckoutRes> CreateCheckoutAsync(
                 PaymongoCheckoutReq req,
                 CancellationToken   ct = default)
            {
        var bookingRef = $"BK-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var payload = new
        {
            data = new
            {
                attributes = new
                {
                    billing = new
                    {
                        name = req.CustomerName,
                        email = req.CustomerEmail,
                        phone = req.CustomerPhone
                    },
                    send_email_receipt = true,
                    show_description = true,
                    show_line_items = true,
                    description = req.Description,
                    statement_descriptor = "PETERBOOKING",
                    line_items = req.LineItems.Select(x => new
                    {
                        currency = "PHP",
                        amount = x.AmountInCentavos,
                        description = x.Description,
                        name = x.Name,
                        quantity = x.Quantity
                    }).ToArray(),
                    payment_method_types = new[]
                    {
                        "qrph"
                    },
                    reference_number = bookingRef,
                    success_url = req.SuccessUrl,
                    cancel_url = req.CancelUrl,
                    metadata = new
                    {
                        booking_reference = bookingRef,
                        customer_name = req.CustomerName,
                        customer_email = req.CustomerEmail,
                        appointment_date = req.AppointmentDate,
                        appointment_time = req.AppointmentTime,
                        description = req.Description,
                        item_count = req.LineItems.Count,
                        total_amount_in_centavos = req.LineItems.Sum(x => x.AmountInCentavos * x.Quantity)
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res     = await httpClient.PostAsync("checkout_sessions", content, ct);
              var resBody = await res.Content.ReadAsStringAsync(ct);

        if (!res.IsSuccessStatusCode)
        {
            return new PaymongoCheckoutRes
            {
                IsSuccess = false,
                BookingReference = bookingRef,
                ErrorMessage = $"PayMongo error: {(int)res.StatusCode} - {resBody}"
            };
        }

        try
        {
            using var doc = JsonDocument.Parse(resBody);

            var data        = doc.RootElement.GetProperty("data");
            var id          = data.GetProperty("id").GetString() ?? string.Empty;
            var attributes  = data.GetProperty("attributes");
            var checkoutUrl = attributes.GetProperty("checkout_url").GetString() ?? string.Empty;

            return new PaymongoCheckoutRes
            {
                IsSuccess = true,
                BookingReference = bookingRef,
                CheckoutSessionId = id,
                CheckoutUrl = checkoutUrl
            };
        }
        catch (Exception ex)
        {
            return new PaymongoCheckoutRes
            {
                IsSuccess = false,
                BookingReference = bookingRef,
                ErrorMessage = $"Failed to parse PayMongo response. {ex.Message}. Raw response: {resBody}"
            };
        }
    }
}