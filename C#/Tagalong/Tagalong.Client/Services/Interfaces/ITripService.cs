using Tagalong.Contracts.DTOs;

namespace Tagalong.Client.Services.Interfaces;

public interface ITripService
{
    Task<List<TripDto>> GetMyTripsAsync();
}
