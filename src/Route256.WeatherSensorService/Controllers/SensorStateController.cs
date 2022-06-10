using Microsoft.AspNetCore.Mvc;
using Route256.WeatherSensorService.Models;

namespace Route256.WeatherSensorService.Controllers;

[Route("state")]
public class SensorStateController : ControllerBase
{
    private readonly IEventStorage _eventStorage;

    public SensorStateController(IEventStorage eventStorage)
    {
        _eventStorage = eventStorage;
    }

    [HttpGet("{id:int}")] //https://localhost:7068/state/1
    public async Task<ActionResult<SensorEvent>> GetSensorReadings(int id)
    {
        if (!_eventStorage.TryGetLastEvent(id, out var sensorEvent))
        {
            return NotFound(id);
        }

        return Ok(sensorEvent);
    }
}