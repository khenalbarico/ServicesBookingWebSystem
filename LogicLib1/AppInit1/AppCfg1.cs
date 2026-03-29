using ToolsLib1.FirebaseClient1;
using ToolsLib1.MailkitSmtp1;
using ToolsLib1.Paymongo1;

namespace LogicLib1.AppInit1;

public class AppCfg1 : 
             IFirebaseCfg,
             IMailkitSmtpCfg,
             IPaymongoCfg
{
    public string ApiKey           { get; set; } = "";
    public string AuthDomain       { get; set; } = "";
    public string DatabaseUrl      { get; set; } = "";
    public string SenderEmail      { get; set; } = "";
    public string SenderName       { get; set; } = "";
    public string AppPassword      { get; set; } = "";
    public string UserName         { get; set; } = "";
    public string SmtpHost         { get; set; } = "";
    public int    SmtpPort         { get; set; }
    public string SecretKey        { get; set; } = "";
    public string BaseUrl          { get; set; } = "";
    public string WebhookSecretKey { get; set; } = "";
}
