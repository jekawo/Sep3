using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tagalong.Contracts.Request;
using WebApi.Extensions;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class VoteController(IActivityService activityService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Vote([FromBody] VoteRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        var result = await activityService.Vote(User.GetUserId(), request.ActivityId, request.Approve!.Value);

        return result.Succeeded ? NoContent() : this.Failure(result);
    }
}
