using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Route256.WeatherSensorClient.Interfaces;
using Route256.WeatherSensorClient.Options;

namespace Route256.WeatherSensorClient.Controllers;

[Route("events")]
public class SensorEventsController : Controller
{
    private readonly IDataStorage _storage;
    private readonly ISubscriptionService _subscriptionService;
    private readonly EventOptions _options;

    public SensorEventsController(IDataStorage storage, ISubscriptionService subscriptionService,
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

    [HttpGet("temperature/{start:datetime}/{periodMin:int}")] //https://localhost:7289/events/temperature/2022-06-10T00:00:00/10
    public async Task<ActionResult<List<double>>> GetAverageTemperatureAsync(DateTime start, int periodMin)
    {
        var result = new List<double>();
        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            var tmp = _storage.GetAverageTemperature(sensor, start, periodMin);
            result.Add(tmp);
        }

        return Ok(result);
    }

    [HttpGet("humidity/{start:datetime}/{periodMin:int}")] //https://localhost:7289/events/humidity/2022-06-10T00:00:00/10
    public async Task<ActionResult<List<double>>> GetAverageHumidityAsync(DateTime start, int periodMin)
    {
        var result = new List<double>();
        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            var hmd = _storage.GetAverageHumidity(sensor, start, periodMin);
            result.Add(hmd);
        }

        return Ok(result);
    }

    [HttpGet("carbon-min/{start:datetime}/{periodMin:int}")] //https://localhost:7289/events/carbon-min/2022-06-10T00:00:00/10
    public async Task<ActionResult<List<int>>> GetMinCarbonDioxideAsync(DateTime start, int periodMin)
    {
        var result = new List<double>();
        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            var cd = _storage.GetMinCarbonDioxide(sensor, start, periodMin);
            result.Add(cd);
        }

        return Ok(result);
    }

    [HttpGet("carbon-max/{start:datetime}/{periodMin:int}")] //https://localhost:7289/events/carbon-max/2022-06-10T00:00:00/10
    public async Task<ActionResult<List<int>>> GetMaxCarbonDioxideAsync(DateTime start, int periodMin)
    {
        var result = new List<double>();
        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            var cd = _storage.GetMaxCarbonDioxide(sensor, start, periodMin);
            result.Add(cd);
        }

        return Ok(result);
    }

    [HttpGet("all")] //https://localhost:7289/events/all
    public async Task<ActionResult<List<IAggregated>>> GetAllSavedDataAsync()
    {
        var result = _storage.GetAllData();
        return Ok(result);
    }

    [HttpPost("{interval:int}")] //https://localhost:7289/events/10
    public async Task<ActionResult> SetAggregationIntervalAsync(int interval)
    {
        _options.AggregationPeriodMin = interval;
        return NoContent();
    }
}