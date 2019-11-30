using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Domain.Telemetry;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace NucuCar.Sensors.Telemetry
{
    public class SensorTelemetry
    {
        public TelemetryPublisher Publisher { get; set; }

        public SensorTelemetry()
        {
            
        }
        
        public SensorTelemetry(ILogger<SensorTelemetry> logger, IOptions<TelemetryConfig> options)
        {
            if (options.Value.ServiceEnabled)
            {
                Publisher = TelemetryPublisherAzure.CreateFromConnectionString(options.Value.ConnectionString,
                    "NucuCar.Sensors", logger);
            }
            else
            {
                Publisher = null;
            }
        }
    }
}