namespace ToolsLib1.Paymongo1;

public class PaymongoCheckoutLineItem
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public int AmountInCentavos { get; set; }
}
