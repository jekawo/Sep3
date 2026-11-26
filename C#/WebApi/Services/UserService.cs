using Tagalong.Contracts.Request;
using WebApi.Gateways.Interfaces;
using WebApi.Helpers;
using WebApi.Models.User;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

public class UserService(IUserGateway users, IRefreshTokenGateway refreshTokens) : IUserService
{
    private const string UsernameTaken = "The username is already taken.";
    private const string UnknownUser = "The user does not exist.";

    public async Task<ServiceResult> Create(CreateUserRequest request)
    {
        // Two simultaneous registrations could both pass this check,
        // so the database should also require usernames to be unique.
        if (await users.GetByUsername(request.Username) is not null)
        {
            return ServiceResult.Conflict(UsernameTaken);
        }

        var (salt, hash) = HashPassword(request.Password);

        await users.Create(new UserCreation(request.Username, request.Email, hash, salt));

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> Update(long id, UpdateUserRequest request)
    {
        var user = await users.GetById(id);

        if (user is null)
        {
            return ServiceResult.NotFound(UnknownUser);
        }

        // A valid token is not enough to change the account, so a stolen token
        // cannot be used to change the password.
        if (!VerifyPassword(request.PreviousPassword, user))
        {
            return ServiceResult.Forbidden();
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var owner = await users.GetByUsername(request.Username);

            if (owner is not null && owner.Id != id)
            {
                return ServiceResult.Conflict(UsernameTaken);
            }

            user.Username = request.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            user.Email = request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            // Assigns both values from the returned tuple at once. A new password also gets a new salt.
            (user.Salt, user.PasswordHash) = HashPassword(request.Password);
        }

        await users.Update(user);

        // After a password change every device has to log in again once its access token expires.
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            await refreshTokens.DeleteAllForUser(id);
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> Delete(long id)
    {
        if (await users.GetById(id) is null)
        {
            return ServiceResult.NotFound(UnknownUser);
        }

        // Tokens first: if this fails, the account still exists and the request can be retried.
        await refreshTokens.DeleteAllForUser(id);
        await users.Delete(id);

        return ServiceResult.Success();
    }

    public async Task<User?> LoginByUsernameAndPassword(string username, string password)
    {
        var user = await users.GetByUsername(username);

        return user is not null && VerifyPassword(password, user)
            ? user
            : null;
    }

    // The salt and hash travel as Base64 strings and are only bytes while hashing.
    private static (string Salt, string Hash) HashPassword(string password)
    {
        var (salt, hash) = PasswordHasher.Hash(password);

        return (Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    private static bool VerifyPassword(string password, User user) => PasswordHasher.Verify(
        password,
        Convert.FromBase64String(user.Salt),
        Convert.FromBase64String(user.PasswordHash));
}
