using Microsoft.AspNetCore.SignalR;
using Tagalong.Grpc;
using Tagalong.Contracts;
using TagalongClient = Tagalong.Grpc.Tagalong.TagalongClient;

namespace WebApi.Services;

public class LiveHub(TagalongClient client) : Hub<IHubCallbacks>, ILiveHub
{
    public async Task<string> SayHello(string name)
    {
        var response = await client.SayHelloAsync(new HelloRequest
        {
            Name = name
        });

        await Clients.Others.ReceiveGreeting(name, response.Message);

        return response.Message;
    }
}
