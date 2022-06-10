using Route256.WeatherSensorClient.Interfaces;

namespace Route256.WeatherSensorClient.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly HashSet<int> _subscribedSensors = new();

    public void SubscribeSensor(int sensorId)
    {
        _subscribedSensors.Add(sensorId);
    }

    public void UnsubscribeSensor(int sensorId)
    {
        _subscribedSensors.Remove(sensorId);
    }

    public IEnumerable<int> GetSubscribedSensors()
    {
        return _subscribedSensors;
    }
}