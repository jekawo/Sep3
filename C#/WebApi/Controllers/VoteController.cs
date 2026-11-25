using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class VoteController(IActivityService activityService) : ControllerBase
{
    [HttpPost]
    public IActionResult Vote([FromBody]object request)
    {
        return Forbid();
    }
}
