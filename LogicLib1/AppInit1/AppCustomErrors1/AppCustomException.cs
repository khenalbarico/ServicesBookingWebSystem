namespace LogicLib1.AppInit1.AppCustomErrors1;

public sealed class AppCustomException(string code, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public string Code { get; } = code;
}
