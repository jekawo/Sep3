using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tagalong.Contracts.Request;
using WebApi.Extensions;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

// "[controller]" is replaced by the class name without "Controller", giving api/users.
// [ApiController] answers 400 by itself when the body cannot be read into the request type.
// Rules checked by our Validate methods answer 422 instead.
[Route("api/[controller]")]
[ApiController]
// The parameters on the class line are a primary constructor: they are injected by
// dependency injection and can be used in every method.
public class UsersController(IUserService userService, IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        var result = await userService.Create(request);

        return result.Succeeded ? Created() : this.Failure(result);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        // "User" is not our User model. It is the property on ControllerBase describing the caller.
        var result = await userService.Update(User.GetUserId(), request);

        return result.Succeeded ? NoContent() : this.Failure(result);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteUser()
    {
        var result = await userService.Delete(User.GetUserId());

        return result.Succeeded ? NoContent() : this.Failure(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        var user = await userService.LoginByUsernameAndPassword(request.Username, request.Password);

        // The same answer for an unknown username and a wrong password,
        // so login does not reveal which of the two was wrong.
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(await authenticationService.CreateLogin(user));
    }

    // No [Authorize] on refresh and logout: the access token has often expired when they are
    // called, and holding the refresh token is what proves who the caller is.
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        var login = await authenticationService.Refresh(request.RefreshToken);

        return login is null ? Unauthorized() : Ok(login);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        // Succeeds even for unknown tokens: either way the caller is logged out.
        await authenticationService.Logout(request.RefreshToken);

        return NoContent();
    }
}
