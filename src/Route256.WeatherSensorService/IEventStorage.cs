using System.Diagnostics.CodeAnalysis;

namespace Route256.WeatherSensorService;

public interface IEventStorage
{
    void AddEvent(long id, IWeatherSensorEvent eventResponse);
    
    bool TryGetEvent(long id, [MaybeNullWhen(false)] out IWeatherSensorEvent eventResponse);
}