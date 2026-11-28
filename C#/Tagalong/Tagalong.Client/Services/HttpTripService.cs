using System.Net.Http.Json;
using Tagalong.Client.Services.Interfaces;
using Tagalong.Contracts.DTOs;

namespace Tagalong.Client.Services;

public class HttpTripService : ITripService
{
    private readonly HttpClient client;

    public HttpTripService(HttpClient client)
    {
        this.client = client;
    }
    public async Task<List<TripDto>> GetMyTripsAsync()
    {
        List<TripDto>? trips = await client.GetFromJsonAsync<List<TripDto>>("/trips");

        return trips ?? new List<TripDto>();
    }
}
