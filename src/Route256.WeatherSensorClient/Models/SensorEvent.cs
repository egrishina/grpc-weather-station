using Route256.WeatherSensorClient.Interfaces;

namespace Route256.WeatherSensorClient.Models;

public class SensorEvent : ISensorEvent
{
    public SensorEvent(long id, int sensorId, double temperature, int humidity, int carbonDioxide, DateTime createdAt)
    {
        Id = id;
        SensorId = sensorId;
        Temperature = temperature;
        Humidity = humidity;
        CarbonDioxide = carbonDioxide;
        CreatedAt = createdAt;
    }

    public long Id { get; }
    public int SensorId { get; }
    public double Temperature { get; }
    public int Humidity { get; }
    public int CarbonDioxide { get; }
    public DateTime CreatedAt { get; }
}