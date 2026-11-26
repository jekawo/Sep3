using System.Net.Http.Json;
using Tagalong.Client.Services.Interfaces;

namespace Tagalong.Client.Services;

public class TagalongHttpClient : ITagalongApi
{
    private readonly HttpClient _httpClient;

    public TagalongHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendActivity(string activity) => _ = await _httpClient.PostAsJsonAsync("/Activity", activity);
}
