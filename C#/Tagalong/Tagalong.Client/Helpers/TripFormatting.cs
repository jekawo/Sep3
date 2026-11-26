using System.Globalization;
using Tagalong.Client.Models;

namespace Tagalong.Client.Helpers;

/// Shared text formatting for trips – used by TripCard, InviteCard, TripSwitcher …
public static class TripFormatting
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture; // "da-DK" for Danish month names

    /// "Berlin, Tyskland · 5 – 7 Dec 2026" – leaves out whatever is missing
    public static string Meta(TripDto trip) =>
        string.Join(" · ", new[] { trip.Destination, DateRange(trip.StartDate, trip.EndDate) }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

    /// "5 – 7 Dec 2026", "28 Jun – 4 Jul 2027", or null when there are no dates
    public static string? DateRange(DateOnly? start, DateOnly? end)
    {
        if (start is null && end is null) return null;
        if (start is null) return end!.Value.ToString("d MMM yyyy", Culture);
        if (end is null) return start.Value.ToString("d MMM yyyy", Culture);

        var s = start.Value;
        var e = end.Value;

        if (s.Month == e.Month && s.Year == e.Year)
            return $"{s.Day} – {e.ToString("d MMM yyyy", Culture)}";

        return $"{s.ToString("d MMM", Culture)} – {e.ToString("d MMM yyyy", Culture)}";
    }
}
