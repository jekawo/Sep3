using WebApi.Gateways.Interfaces;
using WebApi.Models.Trips;

namespace WebApi.Gateways;

// Placeholder until the activity gRPC service exists.
public class UnimplementedActivityGateway : IActivityGateway
{
    private const string Message = "The activity gRPC service is not implemented yet.";

    public Task<TripActivity> Create(ActivityCreation activity) => throw new NotImplementedException(Message);
    public Task<TripActivity?> GetById(long id) => throw new NotImplementedException(Message);
    public Task<IReadOnlyList<TripActivity>> GetForTrip(long tripId) => throw new NotImplementedException(Message);
    public Task Update(TripActivity activity) => throw new NotImplementedException(Message);
}
