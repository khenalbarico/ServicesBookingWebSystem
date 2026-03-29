namespace ToolsLib1.Paymongo1;

public interface IPaymongoCfg
{
    string SecretKey        { get; }
    string BaseUrl          { get; }
    string WebhookSecretKey { get; }
}
