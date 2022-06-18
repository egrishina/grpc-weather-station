using Route256.WeatherSensorClient.Extensions;
using Route256.WeatherSensorClient.Interfaces;
using Route256.WeatherSensorClient.Options;
using Route256.WeatherSensorClient.Services;

namespace Route256.WeatherSensorClient;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<EventOptions>(_configuration.GetSection(EventOptions.Name));
        services.Configure<WeatherSensorServiceOptions>(_configuration.GetSection($"ExternalApis:WeatherSensorService"));
        
        services.AddLocalGrpcClients(_configuration);

        services.AddHostedService<ReceiverHostedService>();
        services.AddSingleton<ISubscriptionService, SubscriptionService>();
        services.AddSingleton<IDataStorage, DataStorage>();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/",
                async context => { await context.Response.WriteAsync("Hello World!"); });
            endpoints.MapControllers();
        });
    }
}