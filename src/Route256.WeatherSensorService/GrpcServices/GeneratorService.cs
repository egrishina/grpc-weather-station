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
        await SendSensorEvent(responseStream, context, Location.Indoors);
        await SendSensorEvent(responseStream, context, Location.Outdoors);
    }

    private async Task SendSensorEvent(
        IServerStreamWriter<EventStreamResponse> responseStream,
        ServerCallContext context,
        Location location)
    {
        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, context.CancellationToken);
                if (_storage.TryGetLastEvent(1, out var result))
                {
                    await responseStream.WriteAsync((EventStreamResponse)result, context.CancellationToken);
                }
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning("The operation was canceled");
        }
    }
}