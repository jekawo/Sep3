using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models.User;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

public class AuthenticationService(IConfiguration configuration) : IAuthenticationService
{
    public Login GenerateJWT(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[Constants.JwtKey]!));
        var expiry = DateTime.UtcNow.AddHours(2);

        var descriptor = new SecurityTokenDescriptor()
        {
            Issuer = configuration[Constants.JwtIssuer],
            Audience = configuration[Constants.JwtAudience],
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            ]),
            Expires = expiry,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new Login
        {
            Token = token,
            Expiry = expiry
        };
    }
}
