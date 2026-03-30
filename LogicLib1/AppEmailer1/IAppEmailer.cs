using LogicLib1.AppModels1.Client;

namespace LogicLib1.AppEmailer1;

public interface IAppEmailer
{
    Task SendEmailAsync(ClientRequest req);
}
