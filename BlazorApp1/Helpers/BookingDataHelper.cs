using LogicLib1.AppModels1.Server.Services;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BlazorApp1.Helpers;

public static class BookingDateHelper
{
    public static List<DateTime> GetAvailableDates(
           this   BaseSvcStructure service,
           int                     daysAhead = 60)
    {
        var result = new List<DateTime>();

        if (service.ScheduleSlots?.IsAvailable != true)
            return result;

        if (service.ScheduleSlots.DaySlots is null || service.ScheduleSlots.DaySlots.Count == 0)
            return result;

        var allowedDays = service.ScheduleSlots.DaySlots
            .Select(NormalizeDayName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < daysAhead; i++)
        {
            var date = DateTime.Now.GetPhilippineNow().Date.AddDays(i);

            if (allowedDays.Contains(date.DayOfWeek.ToString()))
                result.Add(date);
        }

        return result;
    }

    public static string NormalizeDayName(string? rawDay)
    {
        if (string.IsNullOrWhiteSpace(rawDay))
            return "";

        rawDay = rawDay.Trim();

        if (Enum.TryParse<DayOfWeek>(rawDay, true, out var dayOfWeek))
            return dayOfWeek.ToString();

        return rawDay.ToLowerInvariant() switch
        {
            "mon"   => DayOfWeek.Monday.ToString(),
            "tue"   => DayOfWeek.Tuesday.ToString(),
            "tues"  => DayOfWeek.Tuesday.ToString(),
            "wed"   => DayOfWeek.Wednesday.ToString(),
            "thu"   => DayOfWeek.Thursday.ToString(),
            "thur"  => DayOfWeek.Thursday.ToString(),
            "thurs" => DayOfWeek.Thursday.ToString(),
            "fri"   => DayOfWeek.Friday.ToString(),
            "sat"   => DayOfWeek.Saturday.ToString(),
            "sun"   => DayOfWeek.Sunday.ToString(),
            _       => rawDay
        };
    }

    public static DateTime CombineDateAndTime(
           this   DateTime selectedDate,
           string          timeSlot)
    => selectedDate.Date.Add(ParseStartTime(timeSlot));

    public static TimeSpan ParseStartTime(string timeSlot)
    {
        if (string.IsNullOrWhiteSpace(timeSlot))
            return TimeSpan.Zero;

        var normalized = timeSlot.Trim().ToUpperInvariant();

        var rangeMatch = Regex.Match(
            normalized,
            @"^\s*(?<start>\d{1,2}(:\d{2})?)\s*-\s*(?<end>\d{1,2}(:\d{2})?)\s*(?<ampm>AM|PM)\s*$");

        if (rangeMatch.Success)
        {
            var start = rangeMatch.Groups["start"].Value;
            var ampm = rangeMatch.Groups["ampm"].Value;

            if (DateTime.TryParseExact(
                $"{start} {ampm}",
                ["h tt", "hh tt", "h:mm tt", "hh:mm tt"],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dt))
            {
                return dt.TimeOfDay;
            }
        }

        var directMatch = Regex.Match(normalized, @"(?<start>\d{1,2}(:\d{2})?\s*(AM|PM))");

        if (directMatch.Success &&
            DateTime.TryParseExact(
                directMatch.Groups["start"].Value.Replace("  ", " "),
                ["h tt", "hh tt", "h:mm tt", "hh:mm tt"],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            return parsed.TimeOfDay;
        }

        return TimeSpan.Zero;
    }

    public static DateTime GetPhilippineNow(this DateTime dateTime)
    {
        try
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Asia/Manila");
        }
        catch
        {
            try
            {
                return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Singapore Standard Time");
            }
            catch
            {
                return DateTime.UtcNow.AddHours(8);
            }
        }
    }
}