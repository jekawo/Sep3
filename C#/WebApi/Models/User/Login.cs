namespace WebApi.Models.User;

public class Login
{
    public required string Token { get; set; }
    public DateTime Expiry { get; set; }
}
