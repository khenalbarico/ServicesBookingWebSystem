namespace LogicLib1.AppModels1.Client;

public interface IClientService
{
    string   ServiceType        { get; set; }
    string   ServiceDetails     { get; set; }
    decimal  ServiceCost        { get; set; }
    DateTime ServiceSchedule    { get; set; }
}
