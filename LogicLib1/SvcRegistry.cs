using LogicLib1.AppAuth1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolsLib1.FirebaseClient1;
using ToolsLib1.MailkitSmtp1;

namespace LogicLib1;

public static class SvcRegistry
{
    public static void AddSvcRegistry(
        this IServiceCollection svc,
             IConfiguration cfg)
    {
        //AppCfg Registry
        svc.AddAppCfg(cfg);

        //App Logic Registry
        svc.AddScoped<IAppAuthentication, AppAuthentication1>();
        
        //Tool Registry
        svc.AddScoped<IToolAuthEmailProvider, FirebaseAuth1>();
        svc.AddScoped<IToolFirebaseDbOperations, FirebaseRealtimeDb1>();
        svc.AddScoped<IToolSmtpClient, MailkitSmtpClient1>();
    }
}
