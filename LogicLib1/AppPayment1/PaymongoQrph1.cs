using LogicLib1.AppModels1.Client;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToolsLib1.Paymongo1;

namespace LogicLib1.AppPayment1;

public sealed class PaymongoQrph1 (IPaymongoCfg _cfg)
{
    readonly HttpClient HttpClient = _cfg.CreatePaymongoClient();
    public async Task<PaymongoQrphChargeResult> CreateQrphChargeAsync(
                 ClientRequest     payload,
                 CancellationToken ct = default)
    {
        long totalAmountPhp      = payload.ClientServices.Sum(x => x.ServiceCost);
        long totalAmountCentavos = totalAmountPhp * 100;

        string description = BuildDescription(payload);

        var metadata = new Dictionary<string, string>
        {
            ["client_booking_id"] = payload.ClientInformation.ClientBookingId ?? "",
            ["customer_name"] = $"{payload.ClientInformation.FirstName} {payload.ClientInformation.LastName}".Trim(),
            ["customer_email"] = payload.ClientInformation.Email ?? "",
            ["consumer_name"] = $"{payload.ClientInformation.ConsumerFirstName} {payload.ClientInformation.ConsumerLastName}".Trim(),
            ["service_count"] = payload.ClientServices.Count.ToString(),
            ["services"] = string.Join(", ", payload.ClientServices.Select(s => $"{s.ServiceUid}:{s.ServiceName}"))
        };

        var paymentIntentRequest = new
        {
            data = new
            {
                attributes = new
                {
                    amount = totalAmountCentavos,
                    currency = "PHP",
                    capture_type = "automatic",
                    payment_method_allowed = new[] { "qrph" },
                    description,
                    statement_descriptor = "BOOKING",
                    metadata = metadata
                }
            }
        };

        var paymentIntentResponse = await PostAsync<PaymongoEnvelope<PaymentIntentData>>(
            "payment_intents",
            paymentIntentRequest,
            ct);

        var paymentIntentId = paymentIntentResponse.Data.Id;

        var paymentMethodRequest = new
        {
            data = new
            {
                attributes = new
                {
                    type = "qrph",
                    billing = new
                    {
                        name = $"{payload.ClientInformation.FirstName} {payload.ClientInformation.LastName}",
                        email = payload.ClientInformation.Email,
                        phone = payload.ClientInformation.ContactNumber,
                        address = new
                        {
                            line1 = "N/A",
                            line2 = "N/A",
                            city = "N/A",
                            state = "N/A",
                            postal_code = "0000",
                            country = "PH"
                        }
                    },
                }
            }
        };

        var paymentMethodResponse = await PostAsync<PaymongoEnvelope<PaymentMethodData>>(
            "payment_methods",
            paymentMethodRequest,
            ct);

        var paymentMethodId = paymentMethodResponse.Data.Id;

        var attachRequest = new
        {
            data = new
            {
                attributes = new
                {
                    payment_method = paymentMethodId,
                    client_key = paymentIntentResponse.Data.Attributes.ClientKey
                }
            }
        };

        var attachResponse = await PostAsync<PaymongoEnvelope<PaymentIntentData>>(
            $"payment_intents/{paymentIntentId}/attach",
            attachRequest,
            ct);

        var nextAction = attachResponse.Data.Attributes.NextAction;
        var qrCode = nextAction?.Code;

        return new PaymongoQrphChargeResult
        {
            PaymentIntentId = attachResponse.Data.Id,
            PaymentIntentStatus = attachResponse.Data.Attributes.Status,
            PaymentMethodId = paymentMethodId,
            AmountCentavos = attachResponse.Data.Attributes.Amount,
            AmountPhp = attachResponse.Data.Attributes.Amount / 100m,
            QrImageDataUrl = qrCode?.ImageUrl,
            QrCodeId = qrCode?.Id,
            QrLabel = qrCode?.Label,
            NextActionType = nextAction?.Type,
            RawResponse = JsonSerializer.Serialize(attachResponse)
        };
    }

    public async Task<PaymongoEnvelope<PaymentIntentData>> RetrievePaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
            throw new ArgumentException("paymentIntentId is required.", nameof(paymentIntentId));

        using var response = await HttpClient.GetAsync($"payment_intents/{paymentIntentId}", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw CreatePaymongoException(body, response.StatusCode);

        var result = JsonSerializer.Deserialize<PaymongoEnvelope<PaymentIntentData>>(body)
                     ?? throw new InvalidOperationException("Unable to deserialize PayMongo response.");

        return result;
    }

    private async Task<T> PostAsync<T>(string endpoint, object payload, CancellationToken cancellationToken)
    {
        string json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await HttpClient.PostAsync(endpoint, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw CreatePaymongoException(body, response.StatusCode);

        var result = JsonSerializer.Deserialize<T>(body);
        if (result is null)
            throw new InvalidOperationException("Unable to deserialize PayMongo response.");

        return result;
    }

    private static Exception CreatePaymongoException(string responseBody, System.Net.HttpStatusCode statusCode)
    {
        try
        {
            var error = JsonSerializer.Deserialize<PaymongoErrorResponse>(responseBody);
            var first = error?.Errors?.FirstOrDefault();

            if (first is not null)
            {
                return new InvalidOperationException(
                    $"PayMongo error ({(int)statusCode}): {first.Code} - {first.Detail}");
            }
        }
        catch
        {
            // Ignore parsing failure and fall through
        }

        return new InvalidOperationException($"PayMongo error ({(int)statusCode}): {responseBody}");
    }

    private static string BuildDescription(ClientRequest payload)
    {
        var services = string.Join(", ",
            payload.ClientServices.Select(x => $"{x.ServiceName} ({x.ServiceDetails})"));

        return $"Booking {payload.ClientInformation.ClientBookingId}: {services}";
    }
}

#region Result Models

public sealed class PaymongoQrphChargeResult
{
    public string? PaymentIntentId { get; set; }
    public string? PaymentIntentStatus { get; set; }
    public string? PaymentMethodId { get; set; }
    public long AmountCentavos { get; set; }
    public decimal AmountPhp { get; set; }
    public string? QrCodeId { get; set; }
    public string? QrImageDataUrl { get; set; }
    public string? QrLabel { get; set; }
    public string? NextActionType { get; set; }
    public string? RawResponse { get; set; }
}

public sealed class PaymongoEnvelope<T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}

public sealed class PaymentIntentData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public PaymentIntentAttributes Attributes { get; set; } = new();
}

public sealed class PaymentIntentAttributes
{
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("client_key")]
    public string? ClientKey { get; set; }

    [JsonPropertyName("next_action")]
    public PaymongoNextAction? NextAction { get; set; }
}

public sealed class PaymongoNextAction
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("code")]
    public PaymongoQrCode? Code { get; set; }
}

public sealed class PaymongoQrCode
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }
}

public sealed class PaymentMethodData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

public sealed class PaymongoErrorResponse
{
    [JsonPropertyName("errors")]
    public List<PaymongoErrorItem>? Errors { get; set; }
}

public sealed class PaymongoErrorItem
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}

#endregion