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
                Publisher = TelemetryPublisherAzure.CreateFromConnectionString(configuration.ConnectionString,
                    "NucuCar.Sensors", logger);
            }
            else
            {
                Publisher = null;
            }
        }
    }
}