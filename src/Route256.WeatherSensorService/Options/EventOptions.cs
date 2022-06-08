namespace Route256.WeatherSensorService.Options;

public class EventOptions
{
    public const string Name = "EventOptions";

    public int GenerationIntervalMs { get; set; }
    public int AggregationPeriodMin { get; set; }
}