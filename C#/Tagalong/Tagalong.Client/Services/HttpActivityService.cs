using System.Net.Http.Json;
using Tagalong.Contracts.DTOs;
using Tagalong.Client.Services.Interfaces;

namespace Tagalong.Client.Services;

public class HttpActivityService : IActivityService
{
    private readonly HttpClient client;

    public HttpActivityService(HttpClient client)
    {
        this.client = client;
    }
    public async Task<ActivityDto> CreateActivityAsync(
        int tripId, CreateActivityRequest request)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync($"/trips/{tripId}/activity", request);
        
        response.EnsureSuccessStatusCode();

        ActivityDto? activity =
            await response.Content.ReadFromJsonAsync<ActivityDto>();

        return activity ?? throw new Exception("Could not create activity");
    }
}