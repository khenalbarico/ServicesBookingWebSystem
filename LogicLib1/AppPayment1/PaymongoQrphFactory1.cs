using System.Net.Http.Headers;
using System.Text;
using ToolsLib1.Paymongo1;

namespace LogicLib1.AppPayment1;

internal static class PaymongoQrphFactory1
{
    public static HttpClient CreatePaymongoClient(this IPaymongoCfg cfg)
    {
        var basicToken  = Convert.ToBase64String(Encoding.UTF8.GetBytes(cfg.SecretKey));

        var httpClient  = new HttpClient
        {
            BaseAddress = new Uri(cfg.BaseUrl),
        };

        httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Basic",  basicToken);

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        return httpClient;
    }
}
