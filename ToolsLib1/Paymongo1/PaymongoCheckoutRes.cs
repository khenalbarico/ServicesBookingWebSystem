namespace ToolsLib1.Paymongo1;

public class PaymongoCheckoutRes
{
    public bool   IsSuccess         { get; set; }
    public string CheckoutSessionId { get; set; } = "";
    public string CheckoutUrl       { get; set; } = "";
    public string BookingReference  { get; set; } = "";
    public string ErrorMessage      { get; set; } = "";
}
