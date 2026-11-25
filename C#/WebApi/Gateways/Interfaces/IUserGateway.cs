using WebApi.Models.User;

namespace WebApi.Gateways.Interfaces;

public interface IUserGateway
{
    Task<User?> GetById(long id);
    Task<User?> GetByUsername(string username);
    Task<User> Create(UserCreation user);
    Task Update(User user);
    Task Delete(long id);
}
