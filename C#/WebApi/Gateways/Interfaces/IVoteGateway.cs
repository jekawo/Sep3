using WebApi.Models.Trips;

namespace WebApi.Gateways.Interfaces;

public interface IVoteGateway
{
    /// <summary>Stores the vote, replacing any earlier vote by the same user on the same activity.</summary>
    Task Save(Vote vote);
    Task<IReadOnlyList<Vote>> GetForActivity(long activityId);
    Task<IReadOnlyList<Vote>> GetForTrip(long tripId);
}
