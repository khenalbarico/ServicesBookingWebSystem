namespace LogicLib1.AppPayment1;

public sealed class PaymongoQrphChargeRes
{
    public string  PaymentIntentId     { get; set; } = "";
    public string  PaymentIntentStatus { get; set; } = "";
    public string  PaymentMethodId     { get; set; } = "";
    public long    AmountCentavos      { get; set; }
    public decimal AmountPhp           { get; set; }
    public string  QrCodeId            { get; set; } = "";
    public string  QrImageDataUrl      { get; set; } = "";
    public string  QrLabel             { get; set; } = "";
    public string  NextActionType      { get; set; } = "";
    public string  RawResponse         { get; set; } = "";
}
