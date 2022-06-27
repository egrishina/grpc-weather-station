namespace Route256.WeatherSensorService.Models;

public class SensorEvent : ISensorEvent
{
    public long Id { get; set; }

    public int SensorId { get; set; }

    public double Temperature { get; set; }

    public int Humidity { get; set; }

    public int CarbonDioxide { get; set; }

    public DateTime CreatedAt { get; set; }
}