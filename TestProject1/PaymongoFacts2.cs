using LogicLib1.AppInit1;
using LogicLib1.AppModels1.Client;
using LogicLib1.AppPayment1;
using TestProject1.TestTools;
using Xunit.Abstractions;

namespace TestProject1;

public class PaymongoFacts2 (ITestOutputHelper _ctx)
{
    [Fact] public async Task Transact_Paymongo_Qrph()
    {
        var dateTime = new DateTime(2026, 4, 3, 9, 0, 0);

        var payload = new ClientRequest
        {
            ClientInformation = new ClientInformation
            {
                ClientBookingId = "KHE001",
                Email = "khenalbarico05@gmail.com",
                ContactNumber = "+639614854252",
                FirstName = "Khen",
                LastName = "Albarico",
                ConsumerFirstName = "Althea",
                ConsumerLastName = "Austria",
                ConsumerContactNumber = "+639291937211",
                BookingDate = dateTime
            },
            ClientServices =
            [
                new()
                {
                    ServiceUid = "NAS001",
                    ServiceName = "Nails",
                    ServiceDetails = "Nail Extension",
                    ServiceCost = 299,
                    ServiceDate = new DateTime(2026, 4, 3, 9, 0, 0)
                },
                new()
                {
                    ServiceUid = "SPS001",
                    ServiceName = "Spa",
                    ServiceDetails = "Foot spa",
                    ServiceCost = 199,
                    ServiceDate = new DateTime(2026, 4, 5, 11, 0, 0)
                }
            ]
        };

        var cfg = new AppCfg1
        {
            SecretKey        = TestPaymongoAppSettings.SecretKey,
            BaseUrl          = TestPaymongoAppSettings.BaseUrl,
            WebhookSecretKey = TestPaymongoAppSettings.WebhookSecretKey,
        };

        var _sut = new PaymongoQrph1(cfg.SecretKey);

        var res = await _sut.CreateQrphChargeAsync(payload);

        _ctx.WriteLine("Payment Intent ID: {0}", res.PaymentIntentId);
        _ctx.WriteLine("Payment Intent Status: {0}", res.PaymentIntentStatus);
        _ctx.WriteLine("Payment Method ID: {0}", res.PaymentMethodId);
        _ctx.WriteLine("Amount (centavos): {0}", res.AmountCentavos);
        _ctx.WriteLine($"QR Image Data URL: {res.QrImageDataUrl}");
    }
}
