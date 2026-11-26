namespace Tagalong.Contracts.Request;

public class CreateTripRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Destination { get; set; } = string.Empty;
    // Nullable so the validator can tell a missing date apart from a real one.
    // A non-nullable DateOnly would silently become 0001-01-01.
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
