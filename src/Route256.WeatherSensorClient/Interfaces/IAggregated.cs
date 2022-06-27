namespace Route256.WeatherSensorClient.Interfaces;

public interface IAggregated
{
    int SensorId { get; }
    double AverageTemperature { get; }
    double AverageHumidity { get; }
    int MinCarbonDioxide { get; }
    int MaxCarbonDioxide { get; }
    DateTime CreatedAt { get; }
    int ComputedEvents { get; }

    void AggregateEvent(ISensorEvent sensorEvent);
}