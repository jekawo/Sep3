using WebApi.Models.Trips;

namespace WebApi.Gateways.Interfaces;

public interface ITripGateway
{
    /// <summary>Creates the trip with the creator as its first member.</summary>
    Task<Trip> Create(TripCreation trip);
    Task<Trip?> GetById(long id);
    Task<IReadOnlyList<Trip>> GetForUser(long userId);}
