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
            if ((int)(DateTime.UtcNow - last.CreatedAt).TotalMinutes < _options.AggregationPeriodMin)
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

        var temperature = values
            .Where(x => x.CreatedAt > start && (int)(x.CreatedAt - start).TotalMinutes <= periodMin)
            .Select(y => y.AverageTemperature)
            .ToList();

        return temperature.Any() ? temperature.Average() : 0;
    }

    public double GetAverageHumidity(int sensorId, DateTime start, int periodMin)
    {
        if (!_allData.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        var humidity = values
            .Where(x => x.CreatedAt > start && (int)(x.CreatedAt - start).TotalMinutes <= periodMin)
            .Select(y => y.AverageHumidity)
            .ToList();

        return humidity.Any() ? humidity.Average() : 0;
    }

    public int GetMinCarbonDioxide(int sensorId, DateTime start, int periodMin)
    {
        if (!_allData.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        var minCO2 = values
            .Where(x => x.CreatedAt > start && (int)(x.CreatedAt - start).TotalMinutes <= periodMin)
            .Select(y => y.MinCarbonDioxide)
            .ToList();

        return minCO2.Any() ? minCO2.Min() : 0;
    }

    public int GetMaxCarbonDioxide(int sensorId, DateTime start, int periodMin)
    {
        if (!_allData.TryGetValue(sensorId, out var values))
        {
            return 0;
        }

        var maxCO2 = values
            .Where(x => x.CreatedAt > start && (int)(x.CreatedAt - start).TotalMinutes <= periodMin)
            .Select(y => y.MaxCarbonDioxide)
            .ToList();

        return maxCO2.Any() ? maxCO2.Max() : 0;
    }

    public IEnumerable<IAggregated> GetAllData()
    {
        return _allData
            .SelectMany(x => x.Value)
            .OrderBy(y => y.CreatedAt);
    }
}