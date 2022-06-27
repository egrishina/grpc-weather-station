using System.Reflection;

namespace Route256.WeatherSensorClient.Extensions;

public static class ConfigurationExtensions
{
    public static GrpcServiceOptionsBase ReadConfiguredOptions<TOptions>(this IConfiguration configuration)
        where TOptions : GrpcServiceOptionsBase
    {
        var att = typeof(TOptions).GetTypeInfo().GetCustomAttribute(typeof(ConfigSectionAttribute)) as ConfigSectionAttribute;
        return configuration.GetSection(att.Name).Get<GrpcServiceOptionsBase>();
    }
}