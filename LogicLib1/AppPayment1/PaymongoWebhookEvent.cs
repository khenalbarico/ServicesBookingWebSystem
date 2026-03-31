using System.Text.Json.Serialization;

namespace LogicLib1.AppPayment1;

public sealed class PaymongoWebhookEvent
{
    [JsonPropertyName("data")]
    public PaymongoWebhookEventData Data { get; set; } = new();
}

public sealed class PaymongoWebhookEventData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("attributes")]
    public PaymongoWebhookEventAttributes Attributes { get; set; } = new();
}

public sealed class PaymongoWebhookEventAttributes
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("livemode")]
    public bool LiveMode { get; set; }

    [JsonPropertyName("data")]
    public PaymongoWebhookResourceData Data { get; set; } = new();
}

public sealed class PaymongoWebhookResourceData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("attributes")]
    public PaymongoWebhookPaymentAttributes Attributes { get; set; } = new();
}

public sealed class PaymongoWebhookPaymentAttributes
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("payment_intent_id")]
    public string? PaymentIntentId { get; set; }

    [JsonPropertyName("paid_at")]
    public long? PaidAtUnix { get; set; }
}