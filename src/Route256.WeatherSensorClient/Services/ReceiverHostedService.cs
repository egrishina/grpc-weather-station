using Route256.WeatherSensorClient.Interfaces;
using Route256.WeatherSensorClient.Models;
using Route256.WeatherSensorService.EventGenerator;

namespace Route256.WeatherSensorClient.Services;

public class ReceiverHostedService : BackgroundService
{
    private readonly IDataStorage _storage;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IServiceProvider _provider;
    private readonly ILogger<ReceiverHostedService> _logger;

    private bool _waitForChangedSubscribers;
    private CancellationTokenSource _cancellationTokenSource;

    public ReceiverHostedService(IDataStorage storage, ISubscriptionService subscriptionService,
        IServiceProvider provider, ILogger<ReceiverHostedService> logger)
    {
        _storage = storage;
        _subscriptionService = subscriptionService;
        _provider = provider;
        _logger = logger;
        _waitForChangedSubscribers = true;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<Generator.GeneratorClient>();
        _subscriptionService.SubscribersChanged += StopWaiting;

        while (!stoppingToken.IsCancellationRequested)
        {
            _waitForChangedSubscribers = true;
            var waitingTask = WaitForChangedSubscribers();
            var sensors = _subscriptionService.GetSubscribedSensors().ToList();
            if (sensors.Any())
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;
                var receivingTask = ReceiveEventsFromSensors(sensors, client, token);
                await Task.WhenAny(waitingTask, receivingTask);
            }
            else
            {
                await waitingTask;
            }
        }
    }

    private void StopWaiting(object sender, EventArgs e)
    {
        _waitForChangedSubscribers = false;
    }

    private async Task WaitForChangedSubscribers()
    {
        while (_waitForChangedSubscribers)
        {
            await Task.Delay(100);
        }
    }

    private async Task ReceiveEventsFromSensors(IEnumerable<int> sensors, Generator.GeneratorClient client,
        CancellationToken stoppingToken)
    {
        using var stream = client.EventStream(cancellationToken: stoppingToken);
        foreach (var sensor in sensors)
        {
            await stream.RequestStream.WriteAsync(new EventStreamRequest { SensorId = sensor }, stoppingToken);
        }

        await Task.Delay(100, stoppingToken);
        await stream.RequestStream.CompleteAsync();

        while (await stream.ResponseStream.MoveNext(stoppingToken))
        {
            var response = stream.ResponseStream.Current;
            var sensorEvent = new SensorEvent(response.Id, response.SensorId, response.Temperature, response.Humidity,
                response.CarbonDioxide, response.CreatedAt.ToDateTime());

            _storage.AddEvent(sensorEvent.SensorId, sensorEvent);
        }
    }
}