namespace ToolsLib1.Paymongo1;

public interface IToolPaymongo
{
    Task<PaymongoCheckoutRes> CreateCheckoutAsync(
         PaymongoCheckoutReq req,
         CancellationToken   ct = default);
}