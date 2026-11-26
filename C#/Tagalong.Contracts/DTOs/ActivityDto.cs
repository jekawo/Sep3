namespace Tagalong.Contracts.DTOs;
public class ActivityDto
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string? Description { get; set; }
    public DateOnly? Date { get; set; }
    public TimeOnly? StartTime { get; set; } 
    public TimeOnly? EndTime { get; set; }
    public string? Location { get; set; }
    public decimal? PricePerPerson { get; set; }
    public string? Category { get; set; }
}
