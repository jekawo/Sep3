namespace WebApi.Models.User;

public class Login
{
    public required string Token { get; set; }
    public DateTime Expiry { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}
