using WebApi.Gateways.Interfaces;
using WebApi.Models.User;

namespace WebApi.Gateways;

// Placeholder until the refresh token gRPC service exists.
public class UnimplementedRefreshTokenGateway : IRefreshTokenGateway
{
    private const string Message = "The refresh token gRPC service is not implemented yet.";

    public Task Create(StoredRefreshToken token) => throw new NotImplementedException(Message);
    public Task<StoredRefreshToken?> GetByHash(string tokenHash) => throw new NotImplementedException(Message);
    public Task<bool> TryMarkAsUsed(string tokenHash) => throw new NotImplementedException(Message);
    public Task Delete(string tokenHash) => throw new NotImplementedException(Message);
    public Task DeleteAllForUser(long userId) => throw new NotImplementedException(Message);
}
