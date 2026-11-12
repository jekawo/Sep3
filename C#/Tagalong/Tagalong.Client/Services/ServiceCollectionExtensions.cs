using Tagalong.Client.Services.Interfaces;

namespace Tagalong.Client.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the client services. Call from both the WebAssembly and the server host,
    /// since InteractiveAuto components run (and prerender) in either.
    /// </summary>
    public static IServiceCollection AddTagalongClientServices(this IServiceCollection services)
    {
        services.AddHttpClient<ITagalongApi, TagalongHttpClient>();
        return services;
    }
}
