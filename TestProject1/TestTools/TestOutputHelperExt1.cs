using LogicLib1.AppAuth1;
using LogicLib1.AppDb1;
using LogicLib1.AppEmailer1;
using LogicLib1.AppInit1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToolsLib1.FirebaseClient1;
using ToolsLib1.MailkitSmtp1;
using ToolsLib1.Paymongo1;
using Xunit.Abstractions;

namespace TestProject1.TestTools;

internal static class TestOutputHelperExt1
{
    public static T Get<T>(this ITestOutputHelper ctx, AppCfg1 appCfg) where T : class
    {
        using IHost host = new HostBuilder()
            .ConfigureServices(svc =>
            {
                // AppCfg Registry
                svc.AddSingleton<IFirebaseCfg>(appCfg);
                svc.AddSingleton<IMailkitSmtpCfg>(appCfg);
                svc.AddSingleton<IPaymongoCfg>(appCfg);

                // App Logic Registry
                svc.AddScoped<IAppAuthentication, AppAuthentication1>();
                svc.AddScoped<IAppDbOperator, AppDbOperator>();
                svc.AddScoped<IAppEmailer, Emailer1>();

                // Tool Registry
                svc.AddScoped<IToolAuthEmailProvider, FirebaseAuth1>();
                svc.AddScoped<IToolFirebaseDbOperations, FirebaseRealtimeDb1>();
                svc.AddScoped<IToolSmtpClient, MailkitSmtpClient1>();
                svc.AddScoped<IToolPaymongo, Paymongo2>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}