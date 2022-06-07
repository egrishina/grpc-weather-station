using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Route256.WeatherSensorService;

public class EventStorage : IEventStorage
{
    private readonly ConcurrentDictionary<long, IWeatherSensorEvent> _events = new();
    
    public void AddEvent(long id, IWeatherSensorEvent eventResponse)
    {
        _events.AddOrUpdate(id, _ => eventResponse, (_, _) => eventResponse);
    }

    public bool TryGetEvent(long id, [MaybeNullWhen(false)] out IWeatherSensorEvent eventResponse)
    {
        return _events.TryGetValue(id, out eventResponse);
    }
}