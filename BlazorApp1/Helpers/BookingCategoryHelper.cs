namespace BlazorApp1.Helpers;

public static class BookingCategoryHelper
{
    public static string GetDisplayName(this string category) =>
        category.ToLowerInvariant() switch
        {
            "nails"  => "Nails",
            "lashes" => "Lashes",
            "brows"  => "Brows",
            "spas"   => "Spa",
            _        => "Service"
        };
}