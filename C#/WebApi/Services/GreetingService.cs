using Tagalong.Grpc;
using WebApi.Services.Interfaces;
using TagalongClient = Tagalong.Grpc.Tagalong.TagalongClient;

namespace WebApi.Services;

public class GreetingService(TagalongClient client) : IGreetingService
{
    public async Task<string> SayHello(string name)
    {
        var response = await client.SayHelloAsync(new HelloRequest
        {
            Name = name
        });

        return response.Message;
    }
}
