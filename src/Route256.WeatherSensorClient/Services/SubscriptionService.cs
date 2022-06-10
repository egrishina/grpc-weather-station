using Route256.WeatherSensorClient.Interfaces;

namespace Route256.WeatherSensorClient.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly HashSet<int> _subscribedSensors = new();
    
    public event EventHandler? SubscribersChanged;

    public void SubscribeSensor(int sensorId)
    {
        _subscribedSensors.Add(sensorId);
        OnSubscribersChanged(EventArgs.Empty);
    }

    public void UnsubscribeSensor(int sensorId)
    {
        _subscribedSensors.Remove(sensorId);
        OnSubscribersChanged(EventArgs.Empty);
    }

    public IEnumerable<int> GetSubscribedSensors()
    {
        return _subscribedSensors;
    }

    private void OnSubscribersChanged(EventArgs e)
    {
        SubscribersChanged?.Invoke(this, e);
    }
}