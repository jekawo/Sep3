namespace Tagalong.Contracts.DTOs;
public class InviteDto
{
    public int Id { get; set; }
    public TripDto Trip { get; set; } = new();
    public UserSummaryDto InvitedBy { get; set; } = new();
    public string InvitedEmailOrUsername { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
