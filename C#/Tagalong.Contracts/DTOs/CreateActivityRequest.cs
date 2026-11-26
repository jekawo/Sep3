using System.ComponentModel.DataAnnotations;

namespace Tagalong.Contracts.DTOs;
public class CreateActivityRequest
{
    [Required(ErrorMessage = "Activity name is required")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? Date { get; set; } 
    public TimeOnly?  StartTime{ get; set; }
    public TimeOnly?  EndTime{ get; set; }
    public string? Location { get; set; }
    
    [Range(0, double.MaxValue)] 
    public decimal? PricePerPerson { get; set; }
    public string? Category { get; set; }
}
