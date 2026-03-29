namespace ToolsLib1.Paymongo1;

public class PaymongoCheckoutReq
{
    public string ServiceId         { get; set; } = "";
    public string ServiceName       { get; set; } = "";

    public int    AmountInCentavos  { get; set; }

    public string CustomerName      { get; set; } = "";
    public string CustomerEmail     { get; set; } = "";
    public string CustomerPhone     { get; set; } = "";

    public string AppointmentDate   { get; set; } = "";
    public string AppointmentTime   { get; set; } = "";

    public string SuccessUrl        { get; set; } = "";
    public string CancelUrl         { get; set; } = "";

}
