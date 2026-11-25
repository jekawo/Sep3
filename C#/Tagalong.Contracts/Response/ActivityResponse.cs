namespace Tagalong.Contracts.Response;

/// <param name="MyVote">The calling user's vote, or null if they have not voted.</param>
public record ActivityResponse(
    long Id,
    long TripId,
    string Title,
    string? Description,
    string? Location,
    DateTime? StartTime,
    DateTime? EndTime,
    int YesVotes,
    int NoVotes,
    bool? MyVote);
