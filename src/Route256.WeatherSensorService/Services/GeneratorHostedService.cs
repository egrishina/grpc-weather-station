using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Route256.WeatherSensorService.EventGenerator;
using Route256.WeatherSensorService.Models;
using Route256.WeatherSensorService.Options;

namespace Route256.WeatherSensorService.Services;

public class GeneratorHostedService : BackgroundService
{
    private readonly IEventStorage _storage;
    private readonly ILogger<GeneratorHostedService> _logger;
    private readonly EventOptions _options;

    private long _eventId;

    public GeneratorHostedService(IEventStorage storage, ILogger<GeneratorHostedService> logger,
        IOptions<EventOptions> options)
    {
        _storage = storage;
        _logger = logger;
        _options = options.Value;
        _eventId = 0;
    }

    public int GenerationIntervalMs => _options.GenerationIntervalMs;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var task1 = GenerateSensorEvent(stoppingToken, Location.Indoors);
        var task2 = GenerateSensorEvent(stoppingToken, Location.Outdoors);
        await Task.WhenAll(task1, task2);
    }

    private async Task GenerateSensorEvent(CancellationToken stoppingToken, Location location)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(GenerationIntervalMs);
                var result = GetEventResponse(location);
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning("The operation was canceled");
        }
    }

    private EventStreamResponse GetEventResponse(Location location)
    {
        var random = new Random();

        int sensorId;
        double temperature;
        int humidity;
        int carbonDioxide;
        if (location == Location.Indoors)
        {
            sensorId = 1;
            temperature = 18 + random.NextDouble() * 10;
            humidity = random.Next(30, 60);
            carbonDioxide = random.Next(400, 1000);
        }
        else
        {
            sensorId = 2;
            temperature = -10 + random.NextDouble() * 40;
            humidity = random.Next(20, 100);
            carbonDioxide = random.Next(400, 500);
        }

        var id = Interlocked.Increment(ref _eventId);
        var result = new EventStreamResponse
        {
            Id = id,
            SensorId = sensorId,
            Temperature = temperature,
            Humidity = humidity,
            CarbonDioxide = carbonDioxide,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
        };
        _storage.AddEvent(sensorId, result);

        return result;
    }
}