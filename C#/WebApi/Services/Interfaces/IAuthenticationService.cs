using WebApi.Models.User;

namespace WebApi.Services.Interfaces;

public interface IAuthenticationService
{
    /// <summary>Issues a new access token and refresh token for a user whose identity is already confirmed.</summary>
    Task<Login> CreateLogin(User user);

    /// <summary>Exchanges a refresh token for a new login. Returns null if the token cannot be used.</summary>
    Task<Login?> Refresh(string refreshToken);

    Task Logout(string refreshToken);
}
