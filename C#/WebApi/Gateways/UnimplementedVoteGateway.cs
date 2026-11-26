using WebApi.Gateways.Interfaces;
using WebApi.Models.Trips;

namespace WebApi.Gateways;

// Placeholder until the vote gRPC service exists.
public class UnimplementedVoteGateway : IVoteGateway
{
    private const string Message = "The vote gRPC service is not implemented yet.";

    public Task Save(Vote vote) => throw new NotImplementedException(Message);
    public Task<IReadOnlyList<Vote>> GetForActivity(long activityId) => throw new NotImplementedException(Message);
    public Task<IReadOnlyList<Vote>> GetForTrip(long tripId) => throw new NotImplementedException(Message);
}
