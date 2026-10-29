namespace Tagalong.Contracts;

public interface IHubCallbacks
{
    Task ReceiveGreeting(string name, string message);
}
