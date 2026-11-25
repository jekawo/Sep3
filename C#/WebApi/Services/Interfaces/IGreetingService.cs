namespace WebApi.Services.Interfaces;

public interface IGreetingService
{
    Task<string> SayHello(string name);
}
