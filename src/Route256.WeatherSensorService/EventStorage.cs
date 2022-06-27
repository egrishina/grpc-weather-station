using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Route256.WeatherSensorService;

public class EventStorage : IEventStorage
{
    private readonly ConcurrentDictionary<int, List<ISensorEvent>> _events = new();

    public void AddEvent(int sensorId, ISensorEvent sensorEvent)
    {
        _events.AddOrUpdate(sensorId,
            _ => new List<ISensorEvent>() { sensorEvent },
            (_, list) =>
            {
                list.Add(sensorEvent);
                return list;
            });
    }

    public bool TryGetLastEvent(int sensorId, [MaybeNullWhen(false)] out ISensorEvent sensorEvent)
    {
        var result = _events.TryGetValue(sensorId, out var sensorEvents);
        sensorEvent = sensorEvents?.Last();
        return result;
    }

    public bool TryGetAllEvents(int sensorId, [MaybeNullWhen(false)] out List<ISensorEvent> sensorEvents)
    {
        return _events.TryGetValue(sensorId, out sensorEvents);
    }
}