namespace WebApi.Models.User;

public class User
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }

    /// <summary>Base64 encoded.</summary>
    public required string PasswordHash { get; set; }

    /// <summary>Base64 encoded.</summary>
    public required string Salt { get; set; }
}
