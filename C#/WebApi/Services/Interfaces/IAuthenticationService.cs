using WebApi.Models.User;

namespace WebApi.Services.Interfaces;

public interface IAuthenticationService
{
    public Login GenerateJWT(User user);
}
