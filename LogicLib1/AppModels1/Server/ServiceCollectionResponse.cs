using LogicLib1.AppModels1.Server.Services;

namespace LogicLib1.AppModels1.Server;

public class ServiceCollectionResponse
{
    public List<NailService> Nails  { get; set; } = [];
    public List<LashService> Lashes { get; set; } = [];
}