using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NucuCar.Sensors.Abstractions;
using NucuCar.Sensors.Grpc;
using NucuCar.Sensors.Modules.Environment;
using NucuCar.Sensors.Modules.Health;
using NucuCar.Sensors.Modules.Heartbeat;
using NucuCar.Telemetry;

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
                    services.Configure<HeartbeatConfig>(hostContext.Configuration.GetSection("HeartbeatSensor"));

                    // Singletons
                    services.AddSingleton<Telemetry.Telemetry>();
                    services.AddSingleton<ISensor<Bme680Sensor>, Bme680Sensor>();
                    services.AddSingleton<ISensor<CpuTempSensor>, CpuTempSensor>();
                    services.AddSingleton<ISensor<HeartbeatSensor>, HeartbeatSensor>();
                    
                    // Workers
                    services.AddHostedService<TelemetryWorker>();
                    services.AddHostedService<Bme680Worker>();
                    services.AddHostedService<CpuTempWorker>();
                    services.AddHostedService<HeartbeatWorker>();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<GrpcStartup>(); });
    }
}