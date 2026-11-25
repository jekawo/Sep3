namespace Tagalong.Contracts.Response;

public record TripResponse(
    long Id,
    string Name,
    string? Description,
    string Destination,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyCollection<long> MemberIds);
