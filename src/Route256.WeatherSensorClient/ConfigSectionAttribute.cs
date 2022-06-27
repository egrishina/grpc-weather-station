namespace Route256.WeatherSensorClient;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigSectionAttribute : Attribute
{
    public ConfigSectionAttribute(string name)
    {
        Name = name;
    }
    
    public string Name { get; }
}