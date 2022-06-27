namespace Route256.WeatherSensorClient.Interfaces;

public interface ISubscriptionService
{
    event EventHandler SubscribersChanged;
    
    void SubscribeSensor(int sensorId);
    void UnsubscribeSensor(int sensorId);
    IEnumerable<int> GetSubscribedSensors();
}