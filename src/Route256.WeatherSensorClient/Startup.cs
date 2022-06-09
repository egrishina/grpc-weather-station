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
        services.AddGrpcClient<Route256.WeatherSensorService.EventGenerator.Generator.GeneratorClient>(
            options => { options.Address = new Uri("https://localhost:7068"); });

        services.Configure<EventOptions>(_configuration.GetSection(EventOptions.Name));
        services.AddHostedService<ReceiverHostedService>();
        services.AddSingleton<ISubscriptionService, SubscriptionService>();
        services.AddSingleton<IEventStorage, EventStorage>();
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