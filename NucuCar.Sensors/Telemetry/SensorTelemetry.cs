using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors.Telemetry
{
    public class SensorTelemetry
    {
        public TelemetryPublisher Publisher { get; }

        public SensorTelemetry(ILogger<SensorTelemetry> logger, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("Telemetry:Enabled"))
            {
                Publisher = new TelemetryPublisherAzure(new TelemetryPublisherBuilderOptions()
                {
                    ConnectionString = configuration.GetValue<string>("Telemetry:ConnectionString"),
                    TelemetrySource = "NucuCar.Sensors",
                    Logger = logger
                });
            }
            else
            {
                Publisher = null;
            }
        }
    }
}