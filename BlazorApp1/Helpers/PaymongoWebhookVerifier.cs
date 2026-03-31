using System.Security.Cryptography;
using System.Text;

namespace BlazorApp1.Helpers;

public static class PaymongoWebhookVerifier
{
    public static bool IsValid(string rawBody, string signatureHeader, string webhookSecret, bool liveMode)
    {
        if (string.IsNullOrWhiteSpace(rawBody) ||
            string.IsNullOrWhiteSpace(signatureHeader) ||
            string.IsNullOrWhiteSpace(webhookSecret))
            return false;

        var parts = signatureHeader.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split('=', 2))
            .Where(x => x.Length == 2)
            .ToDictionary(x => x[0].Trim(), x => x[1].Trim(), StringComparer.OrdinalIgnoreCase);

        if (!parts.TryGetValue("t", out var timestamp))
            return false;

        var expectedField = liveMode ? "li" : "te";

        if (!parts.TryGetValue(expectedField, out var providedSignature))
            return false;

        var signedPayload = $"{timestamp}.{rawBody}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signedPayload));
        var computedSignature = Convert.ToHexString(hash).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(providedSignature));
    }
}
