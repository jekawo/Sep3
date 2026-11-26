using Tagalong.Contracts.Request;
using Tagalong.Contracts.Response;
using WebApi.Gateways.Interfaces;
using WebApi.Models.Trips;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

public class TripService(ITripGateway trips) : ITripService
{
    public async Task<TripResponse> Create(long userId, CreateTripRequest request)
    {
        // The request has been validated, so both dates are present.
        var trip = await trips.Create(new TripCreation(
            request.Name,
            request.Description,
            request.Destination,
            request.StartDate!.Value,
            request.EndDate!.Value,
            userId));

        return ToResponse(trip);
    }

    public async Task<IReadOnlyList<TripResponse>> GetTripsForUser(long userId, bool inactive)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var userTrips = await trips.GetForUser(userId);

        // "Has the trip ended?" must equal the flag: inactive = true keeps ended trips,
        // inactive = false keeps the rest, including trips ending today (in UTC).
        return userTrips
            .Where(trip => (trip.EndDate < today) == inactive)
            .OrderBy(trip => trip.StartDate)
            .Select(ToResponse)
            .ToList();
    }

    public async Task<ServiceResult<Trip>> GetTripForMember(long userId, long tripId)
    {
        var trip = await trips.GetById(tripId);

        if (trip is null)
        {
            return ServiceResult<Trip>.Failure(ServiceResult.NotFound("The trip does not exist."));
        }

        if (!trip.MemberIds.Contains(userId))
        {
            return ServiceResult<Trip>.Failure(ServiceResult.Forbidden());
        }

        return trip;
    }

    private static TripResponse ToResponse(Trip trip) => new(
        trip.Id,
        trip.Name,
        trip.Description,
        trip.Destination,
        trip.StartDate,
        trip.EndDate,
        trip.MemberIds);
}
