using BlazorApp1.ViewModels;
using LogicLib1.AppModels1.Server;
using LogicLib1.AppModels1.Server.Services;

namespace BlazorApp1.Mappers;

public static class ServiceLandingMapper
{
    public static List<LandingServiceCardVm> ToLandingCards(ServiceCollectionResponse response)
    {
        var result = new List<LandingServiceCardVm>();

        result.AddRange(response.Nails.Select(x => ToCard(x, "Nails", "/images/services/nails.jpg")));
        result.AddRange(response.Lashes.Select(x => ToCard(x, "Lashes", "/images/services/lashes.jpg")));

        return result;
    }

    private static LandingServiceCardVm ToCard(
        BaseSvcStructure svc,
        string category,
        string imageUrl)
    {
        var hasAvailableSchedule = svc.ScheduleSlots?.IsAvailable ?? false;

        var days = svc.ScheduleSlots?.DaySlots?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()
            ?? new List<string>();

        return new LandingServiceCardVm
        {
            Category = category,
            Uid = svc.Uid,
            Title = svc.Details,
            Description = svc.Details,
            Cost = svc.Cost,
            ImageUrl = imageUrl,
            HasAvailableSchedule = hasAvailableSchedule,
            AvailableDays = hasAvailableSchedule ? days : new List<string>()
        };
    }
}