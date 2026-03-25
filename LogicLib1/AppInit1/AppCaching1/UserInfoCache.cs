namespace LogicLib1.AppInit1.AppCaching1;

public class UserInfoCache : IUserInfoCache
{
    public string Email         { get; set; } = "";
    public string FirstName     { get; set; } = "";
    public string LastName      { get; set; } = "";
    public string ContactNumber { get; set; } = "";
}
