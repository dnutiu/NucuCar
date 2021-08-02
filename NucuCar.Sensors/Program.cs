using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NucuCar.Sensors.Abstractions;
using NucuCar.Sensors.Modules.BME680;
using NucuCar.Sensors.Modules.CpuTemperature;
using NucuCar.Sensors.Modules.Heartbeat;
using NucuCar.Sensors.Modules.PMS5003;
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
                    services.Configure<Bme680Config>(hostContext.Configuration.GetSection("Bme680Sensor"));
                    services.Configure<CpuTempConfig>(hostContext.Configuration.GetSection("CpuTemperatureSensor"));
                    services.Configure<HeartbeatConfig>(hostContext.Configuration.GetSection("HeartbeatSensor"));
                    services.Configure<Pms5003Config>(hostContext.Configuration.GetSection("Pms5003Sensor"));

                    // Singletons
                    services.AddSingleton<Telemetry.Telemetry>();
                    services.AddSingleton<ISensor<Bme680Sensor>, Bme680Sensor>();
                    services.AddSingleton<ISensor<CpuTempSensor>, CpuTempSensor>();
                    services.AddSingleton<ISensor<HeartbeatSensor>, HeartbeatSensor>();
                    services.AddSingleton<ISensor<Pms5003Sensor>, Pms5003Sensor>();

                    // Workers
                    // Telemetry
                    services.AddHostedService<TelemetryWorker>();
                    // Sensors
                    services.AddHostedService<Bme680Worker>();
                    services.AddHostedService<CpuTempWorker>();
                    services.AddHostedService<HeartbeatWorker>();
                    services.AddHostedService<Pms5003Worker>();
                });
    }
}