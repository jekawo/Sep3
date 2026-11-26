using Tagalong.Contracts.Request;
using Tagalong.Contracts.Response;
using WebApi.Models.Trips;

namespace WebApi.Services.Interfaces;

public interface ITripService
{
    Task<TripResponse> Create(long userId, CreateTripRequest request);

    /// <param name="userId">The calling user.</param>
    /// <param name="inactive">True returns only trips that have ended, false only those that have not.</param>
    Task<IReadOnlyList<TripResponse>> GetTripsForUser(long userId, bool inactive);

    /// <summary>Returns the trip if it exists and the user is a member of it.</summary>
    Task<ServiceResult<Trip>> GetTripForMember(long userId, long tripId);
}
