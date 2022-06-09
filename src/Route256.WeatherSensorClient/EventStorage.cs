using System.Collections.Concurrent;
using Route256.WeatherSensorClient.Models;

namespace Route256.WeatherSensorClient;

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

    public double GetAverageTemperature(int sensorId, int periodMin)
    {
        if (!_events.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        return values.Cast<SensorEvent>()
            .Where(e => (int)(DateTime.Now - e.CreatedAt).TotalMinutes < periodMin)
            .Average(x => x.Temperature);
    }

    public double GetAverageHumidity(int sensorId, int periodMin)
    {
        if (!_events.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        return values.Cast<SensorEvent>()
            .Where(e => (int)(DateTime.Now - e.CreatedAt).TotalMinutes < periodMin)
            .Average(x => x.Humidity);
    }

    public int GetMinCarbonDioxide(int sensorId, int periodMin)
    {
        if (!_events.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        return values.Cast<SensorEvent>()
            .Where(e => (int)(DateTime.Now - e.CreatedAt).TotalMinutes < periodMin)
            .Min(x => x.CarbonDioxide);
    }

    public int GetMaxCarbonDioxide(int sensorId, int periodMin)
    {
        if (!_events.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        return values.Cast<SensorEvent>()
            .Where(e => (int)(DateTime.Now - e.CreatedAt).TotalMinutes < periodMin)
            .Max(x => x.CarbonDioxide);
    }

    public IEnumerable<ISensorEvent> GetAllEvents()
    {
        return _events.SelectMany(x => x.Value);
    }
}