namespace Route256.WeatherSensorClient.Interfaces;

public interface ISubscriptionService
{
    void SubscribeSensor(int sensorId);
    void UnsubscribeSensor(int sensorId);
    IEnumerable<int> GetSubscribedSensors();
}