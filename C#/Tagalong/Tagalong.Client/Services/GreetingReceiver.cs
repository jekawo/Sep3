using Tagalong.Contracts;

namespace Tagalong.Client.Services;

public sealed class GreetingReceiver(Func<string, string, Task> onGreeting) : IHubCallbacks
{
    public Task ReceiveGreeting(string name, string message) => onGreeting(name, message);
}
