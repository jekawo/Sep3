namespace WebApi.Models.Trips;

public class Trip
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Destination { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<long> MemberIds { get; set; } = [];
}

public record TripCreation(
    string Name,
    string? Description,
    string Destination,
    DateOnly StartDate,
    DateOnly EndDate,
    long CreatorId);
