namespace Tagalong.Client.Models;

public record InviteDto(
    int Id,
    TripDto Trip,
    UserSummaryDto InvitedBy,
    string InvitedEmailOrUsername,
    string Status,
    DateTime CreatedAt);