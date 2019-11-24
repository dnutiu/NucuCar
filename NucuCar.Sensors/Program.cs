using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    var config = hostContext.Configuration;

                    // Singletons
                    services.AddSingleton<Telemetry.SensorTelemetry>();

                    // Workers
                    if (config.GetValue<bool>("Telemetry:Enabled"))
                    {
                        services.AddHostedService<Telemetry.TelemetryBackgroundWorker>();
                    }

                    services.AddHostedService<EnvironmentSensor.BackgroundWorker>();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<GrpcStartup>(); });
    }
}