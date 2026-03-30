using System.Net.Http.Headers;
using System.Text;

namespace ToolsLib1.Paymongo1;

internal static class PaymongoClientFactory1
{
    public static HttpClient CreatePaymongoClient (this IPaymongoCfg cfg)
    {
        var httpClient = new HttpClient();
        var basicToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{cfg.SecretKey}:"));

        httpClient.BaseAddress = new Uri(cfg.BaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicToken);

        if (!httpClient.DefaultRequestHeaders.Accept.Any(h => h.MediaType == "application/json"))      
             httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        return httpClient;
    }
}
