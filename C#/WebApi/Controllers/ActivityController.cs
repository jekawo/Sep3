using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tagalong.Contracts.Request;
using WebApi.Extensions;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ActivityController(IActivityService activityService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddActivity([FromBody] CreateActivityRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        var result = await activityService.Add(User.GetUserId(), request);

        // 201 Created, with a Location header pointing at GetActivityById for the new activity.
        return result.Succeeded
            ? CreatedAtAction(nameof(GetActivityById), new { id = result.Value!.Id }, result.Value)
            : this.Failure(result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateActivity(long id, [FromBody] UpdateActivityRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        var result = await activityService.Update(User.GetUserId(), id, request);

        return result.Succeeded ? Ok(result.Value) : this.Failure(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActivities([FromQuery] long tripId)
    {
        var result = await activityService.GetForTrip(User.GetUserId(), tripId);

        return result.Succeeded ? Ok(result.Value) : this.Failure(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetActivityById(long id)
    {
        var result = await activityService.GetById(User.GetUserId(), id);

        return result.Succeeded ? Ok(result.Value) : this.Failure(result);
    }
}
