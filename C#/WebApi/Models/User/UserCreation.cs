namespace WebApi.Models.User;

/// <param name="PasswordHash">Base64 encoded.</param>
/// <param name="Salt">Base64 encoded.</param>
public record UserCreation(string Username, string Email, string PasswordHash, string Salt);
