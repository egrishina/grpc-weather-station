using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Route256.WeatherSensorClient.Interfaces;
using Route256.WeatherSensorClient.Models;
using Route256.WeatherSensorClient.Options;

namespace Route256.WeatherSensorClient;

public class DataStorage : IDataStorage
{
    private readonly EventOptions _options;
    private readonly ConcurrentDictionary<int, List<IAggregated>> _allData = new();

    public DataStorage(IOptions<EventOptions> options)
    {
        _options = options.Value;
    }

    public void AddEvent(int sensorId, ISensorEvent sensorEvent)
    {
        if (_allData.TryGetValue(sensorId, out var values))
        {
            var last = values.Last();
            if ((int)(DateTime.Now - last.CreatedAt).TotalMinutes < _options.AggregationPeriodMin)
            {
                last.AggregateEvent(sensorEvent);
            }
            else
            {
                values.Add(new AggregatedData(sensorEvent));
            }
        }
        else
        {
            _allData.TryAdd(sensorId, new List<IAggregated>
            {
                new AggregatedData(sensorEvent)
            });
        }
    }

    public double GetAverageTemperature(int sensorId, DateTime start, int periodMin)
    {
        if (!_allData.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        return values
            .Where(x => x.CreatedAt > start && (int)(x.CreatedAt - start).TotalMinutes <= periodMin)
            .Select(y => y.AverageTemperature)
            .Average();
    }

    public double GetAverageHumidity(int sensorId, DateTime start, int periodMin)
    {
        if (!_allData.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        return values
            .Where(x => x.CreatedAt > start && (int)(x.CreatedAt - start).TotalMinutes <= periodMin)
            .Select(y => y.AverageHumidity)
            .Average();
    }

    public int GetMinCarbonDioxide(int sensorId, DateTime start, int periodMin)
    {
        if (!_allData.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        return values
            .Where(x => x.CreatedAt > start && (int)(x.CreatedAt - start).TotalMinutes <= periodMin)
            .Select(y => y.MinCarbonDioxide)
            .Min();
    }

    public int GetMaxCarbonDioxide(int sensorId, DateTime start, int periodMin)
    {
        if (!_allData.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        return values
            .Where(x => x.CreatedAt > start && (int)(x.CreatedAt - start).TotalMinutes <= periodMin)
            .Select(y => y.MaxCarbonDioxide)
            .Max();
    }

    public IEnumerable<IAggregated> GetAllData()
    {
        return _allData.SelectMany(x => x.Value);
    }
}