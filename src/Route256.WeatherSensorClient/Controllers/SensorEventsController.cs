using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Route256.WeatherSensorService.EventGenerator;
using Route256.WeatherSensorService.Models;

namespace Route256.WeatherSensorClient.Controllers;

[Route("events")]
public class SensorEventsController : Controller
{
    private readonly Generator.GeneratorClient _generatorClient;
    private readonly IServiceProvider _provider;
    private readonly ILogger<SensorEventsController> _logger;

    public SensorEventsController(Generator.GeneratorClient generatorClient, IServiceProvider provider,
        ILogger<SensorEventsController> logger)
    {
        _generatorClient = generatorClient;
        _provider = provider;
        _logger = logger;
    }

    [HttpGet("subscribe")] //https://localhost:7289/events/subscribe/?id=1&id=2
    public async Task<ActionResult<SensorEvent>> SubscribeAsync([FromQuery(Name = "id")] List<int> ids,
        CancellationToken cancellationToken)
    {
        try
        {
            using var stream = _generatorClient.EventStream(cancellationToken: cancellationToken);

            while (true)
            {
                //TODO set SensorId from input
                await stream.RequestStream.WriteAsync(new EventStreamRequest { SensorId = 1 }, cancellationToken);

                while (await stream.ResponseStream.MoveNext(cancellationToken))
                {
                    var response = stream.ResponseStream.Current;
                    var sensorEvent = new SensorEvent
                    {
                        Id = response.Id,
                        SensorId = response.SensorId,
                        Temperature = response.Temperature,
                        Humidity = response.Humidity,
                        CarbonDioxide = response.CarbonDioxide,
                        CreatedAt = response.CreatedAt.ToDateTime()
                    };

                    return Ok(sensorEvent);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("The operation was canceled");
        }

        return Ok();
    }

    [HttpGet("unsubscribe")]
    public async Task UnsubscribeAsync([FromQuery(Name = "id")] List<int> ids)
    {
    }
}