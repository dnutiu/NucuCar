using Microsoft.Extensions.Logging;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors.Telemetry
{
    public class SensorTelemetry
    {
        public TelemetryPublisher Publisher { get; }

        public SensorTelemetry(ILogger<SensorTelemetry> logger, TelemetryConfig configuration)
        {
            if (configuration.ServiceEnabled)
            {
                Publisher = new TelemetryPublisherAzure(new TelemetryPublisherBuilderOptions()
                {
                    ConnectionString = configuration.ConnectionString,
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