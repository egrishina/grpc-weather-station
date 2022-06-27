using System.Diagnostics.CodeAnalysis;

namespace Route256.WeatherSensorService;

public interface IEventStorage
{
    void AddEvent(int sensorId, ISensorEvent sensorEvent);
    
    bool TryGetLastEvent(int sensorId, [MaybeNullWhen(false)] out ISensorEvent sensorEvent);

    bool TryGetAllEvents(int sensorId, [MaybeNullWhen(false)] out List<ISensorEvent> sensorEvents);
}