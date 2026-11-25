using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TripController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateTrip([FromBody] object request)
    {
        return Forbid();
    }

    [HttpGet]
    public IActionResult GetTripsForUser([FromQuery] bool inactive)
    {
        return Forbid();
    }
}
