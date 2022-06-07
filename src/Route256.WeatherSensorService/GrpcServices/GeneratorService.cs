using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Route256.WeatherSensorService.EventGenerator;

namespace Route256.WeatherSensorService.GrpcServices;

public class GeneratorService : Generator.GeneratorBase
{
    private readonly IEventStorage _eventStorage;
    private readonly ILogger<GeneratorService> _logger;

    public GeneratorService(IEventStorage eventStorage, ILogger<GeneratorService> logger)
    {
        _eventStorage = eventStorage;
        _logger = logger;
    }

    public override async Task WeatherSensorEventStream(
        Empty request,
        IServerStreamWriter<WeatherSensorEventResponse> responseStream,
        ServerCallContext context)
    {
        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, context.CancellationToken);
                var result = GenerateEvent();
            }
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning("The operation was canceled");
        }
    }

    public override Task<GetWeatherSensorStateResponse> GetWeatherSensorState(
        GetWeatherSensorStateRequest request,
        ServerCallContext context)
    {
        var result = new GetWeatherSensorStateResponse();

        foreach (var id in request.Id)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (_eventStorage.TryGetEvent(id, out var eventResponse))
            {
                result.Result.Add(new WeatherSensorStateResponse
                {
                    Id = id,
                    Temperature = 1,
                    Humidity = 1,
                    CarbonDioxide = 1
                });
            }
        }

        return Task.FromResult(result);
    }

    private WeatherSensorEventResponse GenerateEvent()
    {
        var id = new Random().Next(1, 10);
        var result = new WeatherSensorEventResponse
        {
            Id = id,
            SensorId = 1,
            Temperature = 1,
            Humidity = 1,
            CarbonDioxide = 1
        };
        _eventStorage.AddEvent(id, result);

        return result;
    }
}