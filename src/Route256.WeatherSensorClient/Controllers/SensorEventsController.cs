using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Route256.WeatherSensorClient.Models;
using Route256.WeatherSensorClient.Options;

namespace Route256.WeatherSensorClient.Controllers;

[Route("events")]
public class SensorEventsController : Controller
{
    private readonly IEventStorage _storage;
    private readonly ISubscriptionService _subscriptionService;
    private readonly EventOptions _options;

    public SensorEventsController(IEventStorage storage, ISubscriptionService subscriptionService,
        IOptions<EventOptions> options)
    {
        _storage = storage;
        _subscriptionService = subscriptionService;
        _options = options.Value;
    }

    [HttpGet("subscribe")] //https://localhost:7289/events/subscribe/?id=1&id=2
    public async Task SubscribeAsync([FromQuery(Name = "id")] List<int> ids)
    {
        foreach (var id in ids)
        {
            _subscriptionService.SubscribeSensor(id);
        }
    }

    [HttpGet("unsubscribe")] //https://localhost:7289/events/unsubscribe/?id=1&id=2
    public async Task UnsubscribeAsync([FromQuery(Name = "id")] List<int> ids)
    {
        foreach (var id in ids)
        {
            _subscriptionService.UnsubscribeSensor(id);
        }
    }

    [HttpGet("temperature/{start:datetime}")]
    public async Task<ActionResult<List<double>>> GetAverageTemperatureAsync(DateTime start)
    {
        var periodMin = (int)(DateTime.Now - start).TotalMinutes;
        var result = new List<double>();
        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            var tmp = _storage.GetAverageTemperature(sensor, periodMin);
            result.Add(tmp);
        }

        return Ok(result);
    }

    [HttpGet("humidity/{start:datetime}")]
    public async Task<ActionResult<List<double>>> GetAverageHumidityAsync(DateTime start)
    {
        var periodMin = (int)(DateTime.Now - start).TotalMinutes;
        var result = new List<double>();
        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            var hmd = _storage.GetAverageHumidity(sensor, periodMin);
            result.Add(hmd);
        }

        return Ok(result);
    }

    [HttpGet("carbon-min/{start:datetime}")]
    public async Task<ActionResult<List<int>>> GetMinCarbonDioxideAsync(DateTime start)
    {
        var periodMin = (int)(DateTime.Now - start).TotalMinutes;
        var result = new List<double>();
        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            var cd = _storage.GetMinCarbonDioxide(sensor, periodMin);
            result.Add(cd);
        }

        return Ok(result);
    }

    [HttpGet("carbon-max/{start:datetime}")]
    public async Task<ActionResult<List<int>>> GetMaxCarbonDioxideAsync(DateTime start)
    {
        var periodMin = (int)(DateTime.Now - start).TotalMinutes;
        var result = new List<double>();
        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            var cd = _storage.GetMaxCarbonDioxide(sensor, periodMin);
            result.Add(cd);
        }

        return Ok(result);
    }

    [HttpGet("all")] //https://localhost:7289/events/all
    public async Task<ActionResult<List<SensorEvent>>> GetAllEventsAsync()
    {
        var result = _storage.GetAllEvents().Cast<SensorEvent>();
        return Ok(result);
    }

    [HttpPost("{interval:int}")] //https://localhost:7289/events/10
    public async Task<ActionResult> SetAggregationIntervalAsync(int interval)
    {
        _options.AggregationPeriodMin = interval;
        return NoContent();
    }
}