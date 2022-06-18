using Grpc.Core;

namespace Route256.WeatherSensorClient.Extensions;

public static class ServiceCollectionExtensions
{
    public const int SendMessageSize = 4 * 1024 * 1024;
    public const int ReceiveMessageSize = 4 * 1024 * 1024;
    
    public static void AddLocalGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddLocalGrpcClient<Route256.WeatherSensorService.EventGenerator.Generator.GeneratorClient,
                WeatherSensorServiceOptions>(configuration);
    }

    private static void AddLocalGrpcClient<TClient, TOptions>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TClient : ClientBase
        where TOptions : GrpcServiceOptionsBase
    {
        services.AddGrpcClient<TClient>((
            _,
            config) =>
        {
            var options = configuration.ReadConfiguredOptions<TOptions>();
            var url = options.Url;
            config.Address = new Uri(url!);
            config.ChannelOptionsActions.Add(channelOptions =>
            {
                channelOptions.MaxSendMessageSize = SendMessageSize;
                channelOptions.MaxReceiveMessageSize = ReceiveMessageSize;
            });
        });
    }
}

public class GrpcServiceOptionsBase
{
    public string? Url { get; set; } = null;
}

[ConfigSection("ExternalApis:WeatherSensorService")]
public class WeatherSensorServiceOptions : GrpcServiceOptionsBase
{
}