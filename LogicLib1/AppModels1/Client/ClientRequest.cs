namespace LogicLib1.AppModels1.Client;

public class ClientRequest
{
    public ClientInformation   ClientInformation { get; set; } = new();
    public List<ClientService> ClientServices    { get; set; } = [];
}
