namespace Route256.WeatherSensorClient;

public interface IEventStorage
{
    void AddEvent(int sensorId, ISensorEvent sensorEvent);

    double GetAverageTemperature(int sensorId, int periodMin);

    double GetAverageHumidity(int sensorId, int periodMin);

    int GetMinCarbonDioxide(int sensorId, int periodMin);

    int GetMaxCarbonDioxide(int sensorId, int periodMin);
    
    IEnumerable<ISensorEvent> GetAllEvents();
}