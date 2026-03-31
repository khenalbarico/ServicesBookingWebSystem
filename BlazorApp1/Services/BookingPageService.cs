using BlazorApp1.Helpers;
using LogicLib1.AppDb1;
using LogicLib1.AppModels1.Client;
using LogicLib1.AppModels1.Server.Services;

namespace BlazorApp1.Services;

public class BookingPageService(IAppDbOperator dbOperator)
{
    public async Task<List<BaseSvcStructure>> GetServicesByCategoryAsync(string category)
    {
        var resp = await dbOperator.GetServicesAsync();

        return category.ToLowerInvariant() switch
        {
            "nails" => [.. resp.Nails.Cast<BaseSvcStructure>()],
            "lashes" => [.. resp.Lashes.Cast<BaseSvcStructure>()],
            _ => []
        };
    }

    public ClientRequest BuildClientRequest(
        ClientInformation clientInfo,
        IEnumerable<AppointmentItem> appointmentItems)
    {
        var now = DateTime.Now;
        var bookingId = $"BK-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        return new ClientRequest
        {
            ClientInformation = new ClientInformation
            {
                ClientBookingId = bookingId,
                Email = clientInfo.Email.Trim(),
                ContactNumber = clientInfo.ContactNumber.Trim(),
                FirstName = clientInfo.FirstName.Trim(),
                LastName = clientInfo.LastName.Trim(),
                ConsumerFirstName = clientInfo.ConsumerFirstName.Trim(),
                ConsumerLastName = clientInfo.ConsumerLastName.Trim(),
                ConsumerContactNumber = clientInfo.ConsumerContactNumber.Trim(),
                BookingDate = now.GetPhilippineNow()
            },
            ClientServices = appointmentItems.Select(x => new ClientService
            {
                ServiceUid = x.ServiceUid,
                ServiceName = x.ServiceName,
                ServiceDetails = x.ServiceDetails,
                ServiceCost = (int)x.ServiceCost,
                ServiceDate = x.SelectedDate.CombineDateAndTime(x.SelectedTimeSlot)
            }).ToList()
        };
    }

    public void ApplyConsumerFallbacks(ClientInformation clientInfo)
    {
        var hasAnyConsumerValue =
            !string.IsNullOrWhiteSpace(clientInfo.ConsumerFirstName) ||
            !string.IsNullOrWhiteSpace(clientInfo.ConsumerLastName) ||
            !string.IsNullOrWhiteSpace(clientInfo.ConsumerContactNumber);

        if (!hasAnyConsumerValue)
        {
            clientInfo.ConsumerFirstName = clientInfo.FirstName;
            clientInfo.ConsumerLastName = clientInfo.LastName;
            clientInfo.ConsumerContactNumber = clientInfo.ContactNumber;
        }
    }
}