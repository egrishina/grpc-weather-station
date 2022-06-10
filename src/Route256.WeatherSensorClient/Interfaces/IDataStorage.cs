namespace Route256.WeatherSensorClient.Interfaces;

public interface IDataStorage
{
    void AddEvent(int sensorId, ISensorEvent sensorEvent);

    double GetAverageTemperature(int sensorId, DateTime start, int periodMin);

    double GetAverageHumidity(int sensorId, DateTime start, int periodMin);

    int GetMinCarbonDioxide(int sensorId, DateTime start, int periodMin);

    int GetMaxCarbonDioxide(int sensorId, DateTime start, int periodMin);
    
    IEnumerable<IAggregated> GetAllData();
}