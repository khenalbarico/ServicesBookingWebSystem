using LogicLib1.AppInit1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolsLib1.FirebaseClient1;
using ToolsLib1.MailkitSmtp1;
using ToolsLib1.Paymongo1;

namespace LogicLib1;

public static class AppCfgRegistry
{
    public static void AddAppCfg(
           this IServiceCollection svc,
                IConfiguration     cfg)
    {
        var appCfg = new AppCfg1();

        cfg.GetSection("FirebaseAuthentication").Bind(appCfg);
        cfg.GetSection("MailkitSmtp").Bind(appCfg);
        cfg.GetSection("Paymongo").Bind(appCfg);

        svc.AddSingleton<IFirebaseCfg>(appCfg);
        svc.AddSingleton<IMailkitSmtpCfg>(appCfg);
        svc.AddSingleton<IPaymongoCfg>(appCfg);
    }
}