namespace Tagalong.Contracts.DTOs;
public class TripDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Destination { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public int OrganizerId { get; set; }
    public List<UserSummaryDto> Participants { get; set; } = new();
}
