namespace Route256.WeatherSensorService.Models;

public class WeatherSensorEventResponse : IWeatherSensorEvent
{
    public long Id { get; set; }

    public long SensorId { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public double CarbonDioxide { get; set; }
}