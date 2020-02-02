using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NucuCar.Sensors.Environment;
using NucuCar.Sensors.Grpc;
using NucuCar.Sensors.Health;
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
                    services.Configure<CpuTempConfig>(hostContext.Configuration.GetSection("HealthSensor"));

                    // Singletons
                    services.AddSingleton<SensorTelemetry>();
                    services.AddSingleton<ISensor<Bme680Sensor>, Bme680Sensor>();
                    services.AddSingleton<ISensor<CpuTempSensor>, CpuTempSensor>();
                    
                    // Workers
                    services.AddHostedService<TelemetryWorker>();
                    services.AddHostedService<Bme680Worker>();
                    services.AddHostedService<CpuTempWorker>();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<GrpcStartup>(); });
    }
}