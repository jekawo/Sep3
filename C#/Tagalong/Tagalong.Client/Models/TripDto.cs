using Tagalong.Client.Models;

namespace Tagalong.Client.Models;

public record TripDto(
    int Id,
    string Name,
    string? Destination,
    DateOnly? StartDate,
    DateOnly? EndDate,
    decimal? Budget,
    int OrganizerId,
    List<UserSummaryDto> Participants);