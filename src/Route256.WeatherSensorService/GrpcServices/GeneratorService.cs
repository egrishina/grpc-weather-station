using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Route256.WeatherSensorService.EventGenerator;
using Route256.WeatherSensorService.Models;
using Route256.WeatherSensorService.Options;
using Route256.WeatherSensorService.Services;

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
            //var tasks = new List<Task>();
            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
            {
                var request = requestStream.Current;
                await SendSensorEvent(responseStream, request.SensorId);
                //tasks.Add(SendSensorEvent(responseStream, context, request.SensorId));
            }

            //await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("The operation was canceled");
        }
    }

    private async Task SendSensorEvent(
        IServerStreamWriter<EventStreamResponse> responseStream,
        int sensorId)
    {
        if (_storage.TryGetLastEvent(sensorId, out var sensorEvent))
        {
            await responseStream.WriteAsync((EventStreamResponse)sensorEvent);
        }
        
        // int count = 0;
        // if (_storage.TryGetAllEvents(sensorId, out var sensorEvents) && sensorEvents.Count > count)
        // {
        //     count = sensorEvents.Count;
        //     await responseStream.WriteAsync((EventStreamResponse)sensorEvents[count - 1],
        //         context.CancellationToken);
        // }
    }
}