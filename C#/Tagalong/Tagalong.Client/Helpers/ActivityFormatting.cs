using System.Globalization;
using Tagalong.Contracts.DTOs;

namespace Tagalong.Client.Helpers;

/// Shared text formatting for activities – used by ActivityCard (and later the calendar).
public static class ActivityFormatting
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture; // "da-DK" for Danish day/month names

    /// "Sat 5 Dec · 16:00 – 19:00", "Sat 5 Dec", "16:00", or null when nothing is set
    public static string? When(ActivityDto a)
    {
        var date = a.Date?.ToString("ddd d MMM", Culture);

        string? time = (a.StartTime, a.EndTime) switch
        {
            ({ } s, { } e) => $"{s.ToString("HH:mm", Culture)} – {e.ToString("HH:mm", Culture)}",
            ({ } s, null)  => s.ToString("HH:mm", Culture),
            (null, { } e)  => $"until {e.ToString("HH:mm", Culture)}",
            _              => null
        };

        var parts = new[] { date, time }.Where(p => !string.IsNullOrWhiteSpace(p));
        var result = string.Join(" · ", parts);
        return result.Length == 0 ? null : result;
    }

    /// "Gendarmenmarkt · 25 kr / person", "Free" when price is 0, or null when nothing is set
    public static string? Details(ActivityDto a)
    {
        string? price = a.PricePerPerson switch
        {
            null => null,
            0    => "Free",
            var p => $"{p.Value.ToString("0.##", Culture)} kr / person"
        };

        var parts = new[] { a.Location, price }.Where(p => !string.IsNullOrWhiteSpace(p));
        var result = string.Join(" · ", parts);
        return result.Length == 0 ? null : result;
    }
}
