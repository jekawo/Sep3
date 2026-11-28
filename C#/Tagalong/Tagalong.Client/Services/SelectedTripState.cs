using Tagalong.Contracts.DTOs;

namespace Tagalong.Client.Services;


public class SelectedTripState
{
    public int? ManualTripId { get; private set; }

    public int? DefaultTripId { get; private set; }

    public int? SelectedTripId => ManualTripId ?? DefaultTripId;

    public bool IsManual => ManualTripId is not null;

    public event Action? OnChange;

    public void Select(int tripId)
    {
        if (ManualTripId == tripId) return;
        ManualTripId = tripId;
        OnChange?.Invoke();
    }

    public void Unselect()
    {
        if (ManualTripId is null) return;
        ManualTripId = null;
        OnChange?.Invoke();
    }
    
    public void UpdateTrips(IEnumerable<TripDto> trips)
    {
        var list = trips.ToList();
        var newDefault = SortByDate(list).FirstOrDefault()?.Id;
        var newManual = ManualTripId is not null && list.Any(t => t.Id == ManualTripId) ? ManualTripId : null;

        if (newDefault == DefaultTripId && newManual == ManualTripId) return;

        DefaultTripId = newDefault;
        ManualTripId = newManual;
        OnChange?.Invoke();
    }
    
    public List<TripDto> OrderForDisplay(IEnumerable<TripDto> trips)
    {
        var sorted = SortByDate(trips).ToList();
        if (ManualTripId is null) return sorted;

        var chosen = sorted.FirstOrDefault(t => t.Id == ManualTripId);
        if (chosen is null) return sorted;

        sorted.Remove(chosen);
        sorted.Insert(0, chosen);
        return sorted;
    }

    private static IEnumerable<TripDto> SortByDate(IEnumerable<TripDto> trips) =>
        trips.OrderBy(t => t.StartDate is null)
             .ThenBy(t => t.StartDate)
             .ThenBy(t => t.Name);
}