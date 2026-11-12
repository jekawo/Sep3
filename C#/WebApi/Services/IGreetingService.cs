namespace WebApi.Services;

public interface IGreetingService
{
    Task<string> SayHello(string name);
}
