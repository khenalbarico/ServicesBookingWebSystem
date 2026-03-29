using System.ComponentModel.DataAnnotations;

namespace LogicLib1.AppModels1.Client;

public class ClientRequest : IClientInformation, IClientService
{
    [EmailAddress] public string   Email           { get; set; } = "";

    [Phone]        public string   ContactNumber   { get; set; } = "";
                   public string   FirstName       { get; set; } = "";
                   public string   LastName        { get; set; } = "";
                   public string   ServiceType     { get; set; } = "";
                   public string   ServiceDetails  { get; set; } = "";
                   public decimal  ServiceCost     { get; set; }
                   public DateTime ServiceSchedule { get; set; }
}
