using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tagalong.Contracts;
using WebApi.Services;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Route("/Activity")]
public class Activity(IGreetingService greetings, IHubContext<LiveHub, IHubCallbacks> hub) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateActivity([FromBody] string activity)
    {
        if (string.IsNullOrWhiteSpace(activity))
        {
            return BadRequest();
        }

        var name = activity + " - Activity";
        var message = await greetings.SayHello(name);

        // A hub can't be injected (SignalR creates it per call); IHubContext pushes to connected clients.
        await hub.Clients.All.ReceiveGreeting(name, message);

        return Ok(message);
    }
}
