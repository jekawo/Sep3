namespace WebApi.Models.User;

/// <param name="TokenHash">Base64 encoded hash. The token itself is never stored.</param>
/// <param name="ExpiresAt">UTC.</param>
/// <param name="Used">True once the token has been exchanged for a new one.</param>
public record StoredRefreshToken(long UserId, string TokenHash, DateTime ExpiresAt, bool Used = false);
