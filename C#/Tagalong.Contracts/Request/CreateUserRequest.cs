namespace Tagalong.Contracts.Request;

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Fields left empty are not changed. <see cref="PreviousPassword"/> is always required.
/// </summary>
public class UpdateUserRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string PreviousPassword { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
