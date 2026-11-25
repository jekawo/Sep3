namespace WebApi.Models.Trips;

// Named TripActivity to avoid clashing with System.Diagnostics.Activity and the Activity controller.
public class TripActivity
{
    public long Id { get; set; }
    public long TripId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public record ActivityCreation(
    long TripId,
    string Title,
    string? Description,
    string? Location,
    DateTime? StartTime,
    DateTime? EndTime);
