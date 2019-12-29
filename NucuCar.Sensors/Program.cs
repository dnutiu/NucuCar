using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NucuCar.Sensors.Environment;
using NucuCar.Sensors.Telemetry;

namespace NucuCar.Sensors
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<TelemetryConfig>(hostContext.Configuration.GetSection("Telemetry"));
                    services.Configure<Bme680Config>(hostContext.Configuration.GetSection("EnvironmentSensor"));

                    // Singletons
                    services.AddSingleton<SensorTelemetry>();
                    services.AddSingleton<ISensor<Bme680Sensor>, Bme680Sensor>();
                    
                    // Workers
                    services.AddHostedService<TelemetryWorker>();
                    services.AddHostedService<Bme680Worker>();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<GrpcStartup>(); });
    }
}