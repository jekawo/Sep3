using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tagalong.Contracts.Request;
using WebApi.Extensions;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TripController(ITripService tripService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripRequest request)
    {
        if (!request.Validate(ModelState))
        {
            return UnprocessableEntity(ModelState);
        }

        var trip = await tripService.Create(User.GetUserId(), request);

        return StatusCode(StatusCodes.Status201Created, trip);
    }

    [HttpGet]
    public async Task<IActionResult> GetTripsForUser([FromQuery] bool inactive)
    {
        return Ok(await tripService.GetTripsForUser(User.GetUserId(), inactive));
    }
}
