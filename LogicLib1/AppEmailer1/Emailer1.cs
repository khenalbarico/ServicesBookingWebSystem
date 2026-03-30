using LogicLib1.AppModels1.Client;
using ToolsLib1.MailkitSmtp1;

namespace LogicLib1.AppEmailer1;

public class Emailer1(IToolSmtpClient _emailerClient) : IAppEmailer
{
    const int MaxSendAttempts = 3;

    public async Task SendEmailAsync(ClientRequest req)
    {
        Exception? lastException = null;

        for (var attempt = 1; attempt <= MaxSendAttempts; attempt++)
        {
            try
            {
                await _emailerClient.SendAsync(
                       req.ClientInformation.Email,
                       string.Empty.DefaultSubject(),
                       string.Empty.ComposeMailBody(req));

                return;
            }
            catch (Exception ex) when (attempt < MaxSendAttempts)
            {
                lastException = ex;
            }
            catch (Exception ex)
            {
                lastException = ex;
                break;
            }
        }

        throw new InvalidOperationException(
            $"Failed to send email after {MaxSendAttempts} attempts.",
            lastException);
    }
}