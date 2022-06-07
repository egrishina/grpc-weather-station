using Microsoft.AspNetCore.Mvc;
using Route256.WeatherSensorService.Models;

namespace Route256.WeatherSensorService.Controllers;

[Route("state")]
public class WeatherSensorController : ControllerBase
{
    private readonly IEventStorage _eventStorage;

    public WeatherSensorController(IEventStorage eventStorage)
    {
        _eventStorage = eventStorage;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<WeatherSensorEventResponse>> GetCurrentStateAsync(long id)
    {
        return NoContent();
    }
}