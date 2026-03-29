namespace BlazorApp1.ViewModels;

public class LandingServiceCardVm
{
    public string       Category              { get; set; } = "";
    public string       Uid                   { get; set; } = "";
    public string       Title                 { get; set; } = "";
    public string       Description           { get; set; } = "";
    public decimal      Cost                  { get; set; }
    public string       ImageUrl              { get; set; } = "";
    public bool         HasAvailableSchedule  { get; set; }
    public List<string> AvailableDays         { get; set; } = [];
}