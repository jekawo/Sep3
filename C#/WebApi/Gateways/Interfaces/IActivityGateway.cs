using WebApi.Models.Trips;

namespace WebApi.Gateways.Interfaces;

public interface IActivityGateway
{
    Task<TripActivity> Create(ActivityCreation activity);
    Task<TripActivity?> GetById(long id);
    Task<IReadOnlyList<TripActivity>> GetForTrip(long tripId);
    Task Update(TripActivity activity);
}
