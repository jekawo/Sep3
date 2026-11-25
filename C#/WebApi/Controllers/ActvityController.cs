using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ActvityController : ControllerBase
{
    [HttpPost]
    public IActionResult AddActivity([FromBody] object request)
    {
        return Forbid();
    }
}
