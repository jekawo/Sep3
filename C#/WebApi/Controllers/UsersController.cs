using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tagalong.Contracts.Request;
using WebApi.Extensions;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
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

        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(authenticationService.GenerateJWT(user));
    }
}
