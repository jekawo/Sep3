using WebApi.Gateways.Interfaces;
using WebApi.Models.User;

namespace WebApi.Gateways;

// Placeholder until the user gRPC service exists.
public class UnimplementedUserGateway : IUserGateway
{
    private const string Message = "The user gRPC service is not implemented yet.";

    public Task<User?> GetById(long id) => throw new NotImplementedException(Message);
    public Task<User?> GetByUsername(string username) => throw new NotImplementedException(Message);
    public Task<User> Create(UserCreation user) => throw new NotImplementedException(Message);
    public Task Update(User user) => throw new NotImplementedException(Message);
    public Task Delete(long id) => throw new NotImplementedException(Message);
}
