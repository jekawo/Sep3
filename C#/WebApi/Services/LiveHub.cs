using Microsoft.AspNetCore.SignalR;
using Tagalong.Contracts;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

public class LiveHub(IGreetingService greetings) : Hub<IHubCallbacks>, ILiveHub
{
    public async Task<string> SayHello(string name)
    {
        var message = await greetings.SayHello(name);

        await Clients.Others.ReceiveGreeting(name, message);

        return message;
    }
}
