using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolsLib1.FirebaseClient1;

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

        //Tool Registry
        svc.AddScoped<FirebaseAuth1>();
        svc.AddScoped<FirebaseRealtimeDb1>();

        svc.AddScoped<IToolAuthEmailProvider>(sp => sp.GetRequiredService<FirebaseAuth1>());
        svc.AddScoped<IToolFirebaseDbOperations>(sp => sp.GetRequiredService<FirebaseRealtimeDb1>());

    }
}
