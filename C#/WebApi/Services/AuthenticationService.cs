using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApi.Gateways.Interfaces;
using WebApi.Helpers;
using WebApi.Models.User;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

public class AuthenticationService(
    IConfiguration configuration,
    IUserGateway users,
    IRefreshTokenGateway refreshTokens) : IAuthenticationService
{
    // The access token (JWT) cannot be revoked, so it is short-lived. The client uses the
    // longer-lived refresh token to get a new one without asking for the password again.
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public async Task<Login> CreateLogin(User user)
    {
        // UTC, because token lifetimes are compared in UTC regardless of the server's time zone.
        var now = DateTime.UtcNow;
        var expiry = now.Add(AccessTokenLifetime);
        var refreshExpiry = now.Add(RefreshTokenLifetime);
        var refreshToken = RefreshTokenGenerator.Generate();

        await refreshTokens.Create(new StoredRefreshToken(
            user.Id, RefreshTokenGenerator.Hash(refreshToken), refreshExpiry));

        return new Login
        {
            Token = GenerateJwt(user, expiry),
            Expiry = expiry,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshExpiry
        };
    }

    public async Task<Login?> Refresh(string refreshToken)
    {
        var hash = RefreshTokenGenerator.Hash(refreshToken);
        var stored = await refreshTokens.GetByHash(hash);

        if (stored is null)
        {
            return null;
        }

        // Each refresh token works once. If one comes back a second time, two parties hold it and we
        // cannot know which is the real user, so all of the user's refresh tokens are removed.
        if (stored.Used)
        {
            await refreshTokens.DeleteAllForUser(stored.UserId);
            return null;
        }

        if (stored.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        // False means another request used the token after we read it, which is also reuse.
        if (!await refreshTokens.TryMarkAsUsed(hash))
        {
            await refreshTokens.DeleteAllForUser(stored.UserId);
            return null;
        }

        // Read the user again, so the new access token has the current username.
        var user = await users.GetById(stored.UserId);

        return user is null ? null : await CreateLogin(user);
    }

    public async Task Logout(string refreshToken)
    {
        var hash = RefreshTokenGenerator.Hash(refreshToken);
        var stored = await refreshTokens.GetByHash(hash);

        if (stored is null)
        {
            return;
        }

        // Logging out with an already used token is reuse too.
        if (stored.Used)
        {
            await refreshTokens.DeleteAllForUser(stored.UserId);
            return;
        }

        await refreshTokens.Delete(hash);
    }

    private string GenerateJwt(User user, DateTime expiry)
    {
        // Symmetric: the same secret key both signs the token and verifies it later.
        // For HMAC-SHA256 the key must be at least 32 bytes long.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[Constants.JwtKey]!));

        var descriptor = new SecurityTokenDescriptor()
        {
            Issuer = configuration[Constants.JwtIssuer],
            Audience = configuration[Constants.JwtAudience],
            // Claims are facts about the user, available through "User" in controllers on later requests.
            // The token is signed, not encrypted: anyone holding it can read the claims,
            // but changing them breaks the signature. Never put secrets in here.
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            ]),
            Expires = expiry,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
