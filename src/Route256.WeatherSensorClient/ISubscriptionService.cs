namespace Route256.WeatherSensorClient;

public interface ISubscriptionService
{
    void SubscribeSensor(int sensorId);
    void UnsubscribeSensor(int sensorId);
    IEnumerable<int> GetSubscribedSensors();
}