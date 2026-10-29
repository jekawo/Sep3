namespace Tagalong.Contracts;

public interface ILiveHub
{
    Task<string> SayHello(string name);
}
