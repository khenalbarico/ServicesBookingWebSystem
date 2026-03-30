using System.ComponentModel.DataAnnotations;
using ToolsLib1.FormatAttributes1;

namespace LogicLib1.AppModels1.Client;

public class ClientInformation
{
    [Required]      public string   ClientBookingId       { get; set; } = "";
    [EmailAddress]  public string   Email                 { get; set; } = "";
    [PhPhoneNumber] public string   ContactNumber         { get; set; } = "";
                    public string   FirstName             { get; set; } = "";
                    public string   LastName              { get; set; } = "";
                    public string   ConsumerFirstName     { get; set; } = "";
                    public string   ConsumerLastName      { get; set; } = "";
    [PhPhoneNumber] public string   ConsumerContactNumber { get; set; } = "";
                    public DateTime BookingDate           {  get; set; }
}
