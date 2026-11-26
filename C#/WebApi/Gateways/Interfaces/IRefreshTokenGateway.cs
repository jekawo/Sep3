using WebApi.Models.User;

namespace WebApi.Gateways.Interfaces;

public interface IRefreshTokenGateway
{
    Task Create(StoredRefreshToken token);
    Task<StoredRefreshToken?> GetByHash(string tokenHash);

    /// <summary>
    /// Marks the token as used and returns true, or returns false if it was already used or does not exist.
    /// Must happen as one atomic operation, so two simultaneous requests cannot both get true.
    /// </summary>
    Task<bool> TryMarkAsUsed(string tokenHash);

    Task Delete(string tokenHash);
    Task DeleteAllForUser(long userId);
}
