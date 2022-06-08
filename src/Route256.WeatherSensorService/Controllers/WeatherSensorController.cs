﻿using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SensorEvent>> GetSensorReadings(int id)
    {
        if (_eventStorage.TryGetLastEvent(id, out var sensorEvent))
        {
            return Ok(sensorEvent);
        }
        else
        {
            return NotFound(id);
        }
    }
}