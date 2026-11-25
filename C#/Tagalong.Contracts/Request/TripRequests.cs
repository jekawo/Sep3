namespace Tagalong.Contracts.Request;

public class CreateTripRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Destination { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
