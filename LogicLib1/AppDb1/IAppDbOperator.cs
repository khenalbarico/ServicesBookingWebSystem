using LogicLib1.AppModels1.Client;
using LogicLib1.AppModels1.Server;

namespace LogicLib1.AppDb1;

public interface IAppDbOperator
{
    Task PostBookAsync(ClientRequest req);
    Task<ServiceCollectionResponse> GetServicesAsync();
}
