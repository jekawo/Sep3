using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tagalong.Client.Services;
using Tagalong.Client.Services.Interfaces;

namespace Tagalong.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddTagalongClientServices();

        await builder.Build().RunAsync();
    }
}
