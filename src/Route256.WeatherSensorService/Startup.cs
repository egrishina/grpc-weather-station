using Route256.WeatherSensorService.GrpcServices;
using Route256.WeatherSensorService.Options;
using Route256.WeatherSensorService.Services;

namespace Route256.WeatherSensorService;

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
        services.AddGrpc();
        services.AddHostedService<GeneratorHostedService>();
        services.AddSingleton<IEventStorage, EventStorage>();
        services.AddMvcCore();
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
            endpoints.MapGrpcService<GeneratorService>();
        });
    }
}