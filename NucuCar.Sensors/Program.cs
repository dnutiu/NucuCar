using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NucuCar.Sensors.EnvironmentSensor;
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
                    // Transient
                    services.AddTransient<TelemetryConfig>();
                    services.AddTransient<Bme680Config>();
                    
                    // Singletons
                    services.AddSingleton<SensorTelemetry>();
                    services.AddSingleton<Bme680Sensor>();

                    // Workers
                    services.AddHostedService<TelemetryWorker>();
                    services.AddHostedService<Bme680Worker>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<GrpcStartup>();
                });
    }
}