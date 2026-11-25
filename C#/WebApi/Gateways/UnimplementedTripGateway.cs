using WebApi.Gateways.Interfaces;
using WebApi.Models.Trips;

namespace WebApi.Gateways;

// Placeholder until the trip gRPC service exists.
public class UnimplementedTripGateway : ITripGateway
{
    private const string Message = "The trip gRPC service is not implemented yet.";

    public Task<Trip> Create(TripCreation trip) => throw new NotImplementedException(Message);
    public Task<Trip?> GetById(long id) => throw new NotImplementedException(Message);
    public Task<IReadOnlyList<Trip>> GetForUser(long userId) => throw new NotImplementedException(Message);}
