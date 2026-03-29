using LogicLib1.AppModels1.Client;
using LogicLib1.AppModels1.Server;
using LogicLib1.AppModels1.Server.Services;
using ToolsLib1.FirebaseClient1;

namespace LogicLib1.AppDb1;

public class AppDbOperator (IToolFirebaseDbOperations _dbOperations) : IAppDbOperator
{
    public async Task PostBookAsync(ClientRequest req)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceCollectionResponse> GetServicesAsync()
    {
        var nails  = await _dbOperations.GetListAsync<NailService>("Services", "Nails");

        var lashes = await _dbOperations.GetListAsync<LashService>("Services", "Lash");

        return new ServiceCollectionResponse
        {
            Nails = nails,
            Lashes = lashes
        };
    }
}
