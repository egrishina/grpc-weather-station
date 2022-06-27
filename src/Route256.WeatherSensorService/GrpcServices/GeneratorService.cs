using Grpc.Core;
using Route256.WeatherSensorService.EventGenerator;

namespace Route256.WeatherSensorService.GrpcServices;

public class GeneratorService : Generator.GeneratorBase
{
    private readonly IEventStorage _storage;
    private readonly ILogger<GeneratorService> _logger;

    public GeneratorService(IEventStorage storage, ILogger<GeneratorService> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    public override async Task EventStream(
        IAsyncStreamReader<EventStreamRequest> requestStream,
        IServerStreamWriter<EventStreamResponse> responseStream,
        ServerCallContext context)
    {
        try
        {
            var sensorIds = new List<int>();
            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
            {
                var request = requestStream.Current;
                sensorIds.Add(request.SensorId);
            }

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var tasks = sensorIds.Select(id => SendSensorEvents(responseStream, context, id)).ToList();
                await Task.WhenAll(tasks);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("The operation was canceled");
        }
    }

    private async Task SendSensorEvents(IServerStreamWriter<EventStreamResponse> responseStream,
        ServerCallContext context, int sensorId)
    {
        int count = 0;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            if (_storage.TryGetAllEvents(sensorId, out var sensorEvents) && sensorEvents.Count > count)
            {
                count = sensorEvents.Count;
                await responseStream.WriteAsync((EventStreamResponse)sensorEvents[count - 1],
                    context.CancellationToken);
            }
        }
    }
}