using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tagalong.Contracts.Request;
using WebApi.Extensions;
using WebApi.Helpers;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        var hashes = PasswordHasher.Hash(request.Password);

        // TODO JKW - Map to UserCreation Object
        userService.Create();

        return Created();
    }

    [HttpPut("/{id:long}")]
    public IActionResult UpdateUser(long id, [FromBody] UpdateUserRequest request)
    {
        //TODO JKW - Map to update methods parameters
        userService.Update();
    }
}
