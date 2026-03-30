using LogicLib1.AppEmailer1;
using LogicLib1.AppInit1;
using LogicLib1.AppModels1.Client;
using TestProject1.TestTools;
using Xunit.Abstractions;

namespace TestProject1;

public class MailkitSmtpFacts1 (ITestOutputHelper _ctx)
{
    [Fact] public async Task Send_Email()
    {
        var dateTime          = DateTime.Now;
            dateTime          = new DateTime(2026, 4, 3, 9, 0, 0);

        var cfg = new AppCfg1()
        {
            SenderEmail = TestMailkitSmtpAppSettings.SenderEmail,
            SenderName  = TestMailkitSmtpAppSettings.SenderName,
            AppPassword = TestMailkitSmtpAppSettings.AppPassword,
            UserName    = TestMailkitSmtpAppSettings.UserName,
            SmtpHost    = TestMailkitSmtpAppSettings.SmtpHost,
            SmtpPort    = TestMailkitSmtpAppSettings.SmtpPort

        };

        var payload = new ClientRequest()
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
            ClientServices = new List<ClientService>
            {
                new() {
                    ServiceUid = "NAS001",
                    ServiceName = "Nails",
                    ServiceDetails = "Nail Extension",
                    ServiceCost = 299,
                    ServiceDate = new DateTime(2026, 4, 3, 9, 0, 0)
                },
                new() {
                    ServiceUid = "SPS001",
                    ServiceName = "Spa",
                    ServiceDetails = "Foot spa",
                    ServiceCost = 199,
                    ServiceDate = new DateTime(2026, 4, 5, 11, 0, 0)
                }
            }
        };

        var _sut = _ctx.Get<IAppEmailer>(cfg);

        await _sut.SendEmailAsync(payload);


    }
}
