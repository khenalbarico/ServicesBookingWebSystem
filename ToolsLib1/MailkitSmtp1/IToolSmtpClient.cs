namespace ToolsLib1.MailkitSmtp1;

public interface IToolSmtpClient
{
    Task SendAsync(
        string recepient,
        string subject = "",
        string body    = "");
}
