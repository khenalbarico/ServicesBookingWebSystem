using System.Net;
using System.Text;
using LogicLib1.AppModels1.Client;

namespace LogicLib1.AppEmailer1;

internal static class MailFactory1
{
    public static string ComposeMailBody(
        this string body,
        ClientRequest req)
    {
        var client = req.ClientInformation;
        var services = req.ClientServices ?? new List<ClientService>();

        var fullName = $"{client?.FirstName} {client?.LastName}".Trim();
        var consumerName = $"{client?.ConsumerFirstName} {client?.ConsumerLastName}".Trim();
        var totalCost = services.Sum(x => x.ServiceCost);

        var bookingId = client?.ClientBookingId ?? string.Empty;
        var bookingDate = client?.BookingDate ?? DateTime.MinValue;

        var servicesHtml = new StringBuilder();

        foreach (var service in services.OrderBy(x => x.ServiceDate))
        {
            servicesHtml.Append($@"
                <tr>
                    <td style='padding:16px; border-bottom:1px solid #d9d9d9;'>
                        <div style='font-size:16px; font-weight:bold; color:#000000; margin-bottom:6px;'>
                            {Encode(service.ServiceName)}
                        </div>
                        <div style='font-size:14px; color:#444444; margin-bottom:8px;'>
                            {Encode(service.ServiceDetails)}
                        </div>
                        <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0'>
                            <tr>
                                <td style='font-size:13px; color:#555555; padding:2px 0;'>
                                    <strong>Reference:</strong> {Encode(service.ServiceUid)}
                                </td>
                            </tr>
                            <tr>
                                <td style='font-size:13px; color:#555555; padding:2px 0;'>
                                    <strong>Date:</strong> {service.ServiceDate:MMMM d, yyyy}
                                </td>
                            </tr>
                            <tr>
                                <td style='font-size:13px; color:#555555; padding:2px 0;'>
                                    <strong>Time:</strong> {service.ServiceDate:h:mm tt}
                                </td>
                            </tr>
                            <tr>
                                <td style='font-size:13px; color:#555555; padding:2px 0;'>
                                    <strong>Price:</strong> ₱{service.ServiceCost:N2}
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>");
        }

        var appointmentSummary = services.Any()
            ? string.Join("<br/>", services
                .OrderBy(x => x.ServiceDate)
                .Select(x => $"{Encode(x.ServiceName)} - {serviceDateDisplay(x.ServiceDate)}"))
            : "No services selected.";

        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>ABG Studios Service</title>
</head>
<body style='margin:0; padding:0; background-color:#f3f3f3; font-family:Arial, Helvetica, sans-serif; color:#000000;'>
    <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f3f3f3; margin:0; padding:24px 0;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width:720px; background-color:#ffffff; border:1px solid #000000;'>
                    
                    <tr>
                        <td style='background-color:#000000; padding:28px 32px; text-align:center;'>
                            <div style='font-size:28px; font-weight:bold; color:#ffffff; letter-spacing:1px;'>
                                ABG STUDIOS
                            </div>
                            <div style='font-size:13px; color:#d9d9d9; margin-top:8px;'>
                                Service Booking Confirmation
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:32px;'>

                            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='margin-bottom:24px;'>
                                <tr>
                                    <td style='background-color:#000000; color:#ffffff; padding:14px 18px; font-size:14px; text-align:left;'>
                                        <strong>Booking ID:</strong> {Encode(bookingId)}
                                    </td>
                                </tr>
                            </table>

                            <div style='font-size:22px; font-weight:bold; color:#000000; margin-bottom:16px;'>
                                Hello {Encode(fullName)},
                            </div>

                            <div style='font-size:15px; line-height:1.7; color:#222222; margin-bottom:24px;'>
                                Thank you for booking with <strong>ABG Studios</strong>.
                                We have received your service request{(services.Count > 1 ? "s" : "")} and prepared the summary below.
                            </div>

                            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='border:1px solid #d9d9d9; margin-bottom:28px;'>
                                <tr>
                                    <td style='background-color:#fafafa; padding:16px 20px; border-bottom:1px solid #d9d9d9; font-size:16px; font-weight:bold; color:#000000;'>
                                        Booking Information
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding:20px;'>
                                        <div style='font-size:14px; color:#333333; margin-bottom:8px;'>
                                            <strong>Booking ID:</strong> {Encode(bookingId)}
                                        </div>
                                        <div style='font-size:14px; color:#333333; margin-bottom:8px;'>
                                            <strong>Booking Date:</strong> {bookingDateDisplay(bookingDate)}
                                        </div>
                                        <div style='font-size:14px; color:#333333;'>
                                            <strong>Total Services:</strong> {services.Count}
                                        </div>
                                    </td>
                                </tr>
                            </table>

                            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='border:1px solid #d9d9d9; margin-bottom:28px;'>
                                <tr>
                                    <td style='background-color:#fafafa; padding:16px 20px; border-bottom:1px solid #d9d9d9; font-size:16px; font-weight:bold; color:#000000;'>
                                        Client Information
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding:20px;'>
                                        <div style='font-size:14px; color:#333333; margin-bottom:8px;'>
                                            <strong>Client Name:</strong> {Encode(fullName)}
                                        </div>
                                        <div style='font-size:14px; color:#333333; margin-bottom:8px;'>
                                            <strong>Email:</strong> {Encode(client?.Email)}
                                        </div>
                                        <div style='font-size:14px; color:#333333; margin-bottom:8px;'>
                                            <strong>Contact Number:</strong> {Encode(client?.ContactNumber)}
                                        </div>
                                        <div style='font-size:14px; color:#333333;'>
                                            <strong>Consumer:</strong> {Encode(consumerName)} ({Encode(client?.ConsumerContactNumber)})
                                        </div>
                                    </td>
                                </tr>
                            </table>

                            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='border:1px solid #d9d9d9; margin-bottom:28px;'>
                                <tr>
                                    <td style='background-color:#fafafa; padding:16px 20px; border-bottom:1px solid #d9d9d9; font-size:16px; font-weight:bold; color:#000000;'>
                                        Booked Services
                                    </td>
                                </tr>
                                {servicesHtml}
                            </table>

                            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='margin-bottom:28px;'>
                                <tr>
                                    <td style='background-color:#000000; color:#ffffff; padding:18px 20px; font-size:18px; font-weight:bold; text-align:right;'>
                                        Total Amount: ₱{totalCost:N2}
                                    </td>
                                </tr>
                            </table>

                            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='border:1px solid #d9d9d9; margin-bottom:28px;'>
                                <tr>
                                    <td style='background-color:#fafafa; padding:16px 20px; border-bottom:1px solid #d9d9d9; font-size:16px; font-weight:bold; color:#000000;'>
                                        Appointment Summary
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding:20px; font-size:14px; color:#333333; line-height:1.8;'>
                                        {appointmentSummary}
                                    </td>
                                </tr>
                            </table>

                            <div style='font-size:14px; line-height:1.7; color:#333333; margin-bottom:24px;'>
                                Please keep this email for your reference. If you need to update your booking details,
                                kindly contact our team as soon as possible.
                            </div>

                            <div style='font-size:14px; line-height:1.7; color:#333333;'>
                                Regards,<br/>
                                <strong>ABG Studios</strong>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td style='background-color:#000000; padding:18px 24px; text-align:center;'>
                            <div style='font-size:12px; color:#ffffff;'>
                                © {DateTime.UtcNow.Year} ABG Studios. All rights reserved.
                            </div>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    public static string DefaultSubject(this string subject)
        => "ABG Studios Service";

    private static string Encode(string? value)
        => WebUtility.HtmlEncode(value ?? string.Empty);

    private static string bookingDateDisplay(DateTime value)
        => value == DateTime.MinValue
            ? "N/A"
            : $"{value:MMMM d, yyyy}, {value:dddd}, {value:h:mm tt}";

    private static string serviceDateDisplay(DateTime value)
        => $"{value:MMMM d, yyyy}, {value:dddd}, {value:h:mm tt}";
}