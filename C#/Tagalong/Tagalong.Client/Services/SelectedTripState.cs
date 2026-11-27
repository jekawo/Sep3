using Tagalong.Contracts.DTOs;

namespace Tagalong.Client.Services;

// Remembers which trip is selected. Shared by My trips (TripCard "Select") and
// the Activities pages (TripSwitcher + the Activities tab link).

// Rules:
// - Nothing chosen yet → the soonest trip is selected (trips without dates last).
// - The selected trip is always shown FIRST (My trips list and TripSwitcher list),
//   the others follow by start date, soonest first.

public class SelectedTripState
{
    public int? SelectedTripId { get; private set; }

    /// <summary>Raised when the selection changes – pages subscribe and call StateHasChanged.</summary>
    public event Action? OnChange;

    /// <summary>Select a trip (from TripCard "Select" or TripSwitcher).</summary>
    public void Select(int tripId)
    {
        if (SelectedTripId == tripId) return;
        SelectedTripId = tripId;
        OnChange?.Invoke();
    }

    /// <summary>Forget the selection, e.g. after the selected trip was deleted or on log out.</summary>
    public void Clear()
    {
        if (SelectedTripId is null) return;
        SelectedTripId = null;
        OnChange?.Invoke();
    }

    /// <summary>
    /// Call right after loading the user's trips. Keeps the current selection if that trip
    /// still exists; otherwise selects the soonest trip. Does nothing when there are no trips.
    /// </summary>
    public void EnsureDefault(IEnumerable<TripDto> trips)
    {
        var list = trips.ToList();

        if (SelectedTripId is not null && list.Any(t => t.Id == SelectedTripId))
            return;

        var soonest = SortByDate(list).FirstOrDefault();
        if (soonest is not null)
            Select(soonest.Id);
        else
            Clear();
    }

    /// <summary>
    /// The order to show trips in: the selected trip first, then the rest soonest first.
    /// Use this on My trips and for the TripSwitcher list.
    /// </summary>
    public List<TripDto> OrderForDisplay(IEnumerable<TripDto> trips)
    {
        var sorted = SortByDate(trips).ToList();

        var selected = sorted.FirstOrDefault(t => t.Id == SelectedTripId);
        if (selected is null) return sorted;

        sorted.Remove(selected);
        sorted.Insert(0, selected);
        return sorted;
    }

    /// Soonest start date first; trips without a start date last, then by name.
    private static IEnumerable<TripDto> SortByDate(IEnumerable<TripDto> trips) =>
        trips.OrderBy(t => t.StartDate is null)
             .ThenBy(t => t.StartDate)
             .ThenBy(t => t.Name);
}
