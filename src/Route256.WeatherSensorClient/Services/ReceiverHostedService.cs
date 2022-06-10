using Route256.WeatherSensorClient.Interfaces;
using Route256.WeatherSensorClient.Models;
using Route256.WeatherSensorService.EventGenerator;

namespace Route256.WeatherSensorClient.Services;

public class ReceiverHostedService  : BackgroundService
{
    private readonly IDataStorage _storage;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IServiceProvider _provider;

    public ReceiverHostedService(IDataStorage storage, ISubscriptionService subscriptionService,
        IServiceProvider provider)
    {
        _storage = storage;
        _subscriptionService = subscriptionService;
        _provider = provider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<Generator.GeneratorClient>();
        using var stream = client.EventStream(cancellationToken: stoppingToken);

        var sensors = _subscriptionService.GetSubscribedSensors();
        foreach (var sensor in sensors)
        {
            await stream.RequestStream.WriteAsync(new EventStreamRequest { SensorId = sensor }, stoppingToken);
        }

        while (await stream.ResponseStream.MoveNext(stoppingToken))
        {
            var response = stream.ResponseStream.Current;
            var sensorEvent = new SensorEvent(response.Id, response.SensorId, response.Temperature, response.Humidity,
                response.CarbonDioxide, response.CreatedAt.ToDateTime());

            _storage.AddEvent(sensorEvent.SensorId, sensorEvent);
        }
    }
}