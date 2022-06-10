namespace Route256.WeatherSensorClient.Interfaces;

public interface ISensorEvent
{
    long Id { get; }
    int SensorId { get; }
    double Temperature { get; }
    int Humidity { get; }
    int CarbonDioxide { get; }
    DateTime CreatedAt { get; }
}