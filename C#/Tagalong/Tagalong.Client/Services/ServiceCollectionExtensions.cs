using Tagalong.Client.Services.Interfaces;

namespace Tagalong.Client.Services;

public static class ServiceCollectionExtensions
{
    private static readonly Uri ApiBaseAddress =
        new Uri("https://localhost:7092");

    // Registers the client services. Call from both the WebAssembly and the server host,
    
    public static IServiceCollection AddTagalongClientServices(
        this IServiceCollection services)
    {
        services.AddHttpClient<ITagalongApi, TagalongHttpClient>(client =>
            client.BaseAddress = ApiBaseAddress);
        services.AddHttpClient<IActivityService, HttpActivityService>(client =>
            client.BaseAddress = ApiBaseAddress);
        services.AddHttpClient<ITripService, HttpTripService>(client =>
            client.BaseAddress = ApiBaseAddress);
        services.AddHttpClient<IInviteService, HttpInviteService>(client =>
            client.BaseAddress = ApiBaseAddress);
        services.AddScoped<SelectedTripState>();
        return services;
    }
}