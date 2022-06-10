using Route256.WeatherSensorClient.Interfaces;

namespace Route256.WeatherSensorClient.Models;

public class AggregatedData : IAggregated
{
    public AggregatedData(ISensorEvent sensorEvent)
    {
        SensorId = sensorEvent.SensorId;
        AverageTemperature = sensorEvent.Temperature;
        AverageHumidity = sensorEvent.Humidity;
        MinCarbonDioxide = sensorEvent.CarbonDioxide;
        MaxCarbonDioxide = sensorEvent.CarbonDioxide;
        CreatedAt = sensorEvent.CreatedAt;
        ComputedEvents = 1;
    }

    public int SensorId { get; }
    public double AverageTemperature { get; private set; }
    public double AverageHumidity { get; private set; }
    public int MinCarbonDioxide { get; private set; }
    public int MaxCarbonDioxide { get; private set; }
    public DateTime CreatedAt { get; }
    public int ComputedEvents { get; private set; }

    public void AggregateEvent(ISensorEvent sensorEvent)
    {
        if (SensorId != sensorEvent.SensorId)
        {
            throw new ArgumentException("SensorId of the passed event differs from already aggregated events.",
                nameof(sensorEvent));
        }

        AverageTemperature = (AverageTemperature * ComputedEvents + sensorEvent.Temperature) / (ComputedEvents + 1);
        AverageHumidity = (AverageHumidity * ComputedEvents + sensorEvent.Humidity) / (ComputedEvents + 1);
        MinCarbonDioxide = Math.Min(MinCarbonDioxide, sensorEvent.CarbonDioxide);
        MaxCarbonDioxide = Math.Max(MinCarbonDioxide, sensorEvent.CarbonDioxide);
        ComputedEvents += 1;
    }
}