using LogicLib1.AppInit1;
using LogicLib1.AppModels1.Client;
using LogicLib1.AppPayment1;
using TestProject1.TestTools;
using ToolsLib1.Paymongo1;
using Xunit.Abstractions;

namespace TestProject1;

public class PaymongoFacts1(ITestOutputHelper _ctx)
{
    [Fact] public async Task Checkout_Page_Paymongo()
    {
        var dateTime = new DateTime(2026, 4, 3, 9, 0, 0);

        var payload  = new ClientRequest
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

        var sut = _ctx.Get<IToolPaymongo>(cfg);

        var res = await sut.CreateCheckoutAsync(payload.BuildCheckoutRequest());

        _ctx.WriteLine($"IsSuccess: {res.IsSuccess}");
        _ctx.WriteLine($"CheckoutSessionId: {res.CheckoutSessionId}");
        _ctx.WriteLine($"CheckoutUrl: {res.CheckoutUrl}");
    }
}