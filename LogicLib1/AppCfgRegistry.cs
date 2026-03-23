using LogicLib1.AppInit1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolsLib1.FirebaseClient1;

namespace LogicLib1;

public static class AppCfgRegistry
{
    public static void AddAppCfg(
        this IServiceCollection svc,
             IConfiguration cfg)
    {
        var appCfg = new AppCfg1();
        cfg.GetSection("FirebaseAuthentication").Bind(appCfg);

        svc.AddSingleton<IFirebaseCfg>(appCfg);
    }
}
