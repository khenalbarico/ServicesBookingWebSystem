namespace LogicLib1.AppModels1.Client;

public sealed class AppointmentItem
{
    public string   ServiceUid       { get; set; } = "";
    public string   ServiceName      { get; set; } = "";
    public string   ServiceDetails   { get; set; } = "";
    public decimal  ServiceCost      { get; set; }
    public DateTime SelectedDate     { get; set; }
    public string   SelectedTimeSlot { get; set; } = "";
}