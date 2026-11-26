using Tagalong.Contracts.Request;
using WebApi.Models.User;

namespace WebApi.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResult> Create(CreateUserRequest request);
    Task<ServiceResult> Update(long id, UpdateUserRequest request);
    Task<ServiceResult> Delete(long id);
    Task<User?> LoginByUsernameAndPassword(string username, string password);
}
