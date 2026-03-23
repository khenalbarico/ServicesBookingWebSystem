using ToolsLib1.FirebaseClient1;

namespace LogicLib1.AppInit1;

public class AppCfg1 : IFirebaseCfg
{
    public string FirebaseApiKey      { get; set; } = "";
    public string FirebaseAuthDomain  { get; set; } = "";
    public string FirebaseDatabaseUrl { get; set; } = "";
}
