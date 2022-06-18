using Grpc.Core;
using Route256.WeatherSensorClient.Helpers;
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
    private IEnumerable<int> _sensors;
    private Generator.GeneratorClient _client;

    public ReceiverHostedService(IDataStorage storage, ISubscriptionService subscriptionService,
        IServiceProvider provider, ILogger<ReceiverHostedService> logger)
    {
        _storage = storage;
        _subscriptionService = subscriptionService;
        _provider = provider;
        _logger = logger;
        _waitForChangedSubscribers = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _sensors = _subscriptionService.GetSubscribedSensors().ToList();
        _subscriptionService.SubscribersChanged += StopWaiting;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        _client = scope.ServiceProvider.GetRequiredService<Generator.GeneratorClient>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var waitingTask = WaitForChangedSubscribers();
            var receivingTask = RetryHelper.DoWithRetryAsync(ReceiveEventsFromSensors, _logger,
                RetryHelper.DefaultRetryCount, ex => ex is RpcException { StatusCode: StatusCode.Unavailable },
                RetryHelper.DefaultRetryDelay);

            await Task.WhenAny(waitingTask, receivingTask);
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

        _waitForChangedSubscribers = true;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _sensors = _subscriptionService.GetSubscribedSensors().ToList();
    }

    private async Task<bool> ReceiveEventsFromSensors()
    {
        using var stream = _client.EventStream(cancellationToken: _cancellationTokenSource.Token);
        foreach (var sensor in _sensors)
        {
            await stream.RequestStream.WriteAsync(new EventStreamRequest { SensorId = sensor },
                _cancellationTokenSource.Token);
        }

        await Task.Delay(100, _cancellationTokenSource.Token);
        await stream.RequestStream.CompleteAsync();

        while (await stream.ResponseStream.MoveNext(_cancellationTokenSource.Token))
        {
            var response = stream.ResponseStream.Current;
            var sensorEvent = new SensorEvent(response.Id, response.SensorId, response.Temperature, response.Humidity,
                response.CarbonDioxide, response.CreatedAt.ToDateTime());

            _storage.AddEvent(sensorEvent.SensorId, sensorEvent);
        }

        return true;
    }
}