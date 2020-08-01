using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Telemetry.Abstractions;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace NucuCar.Telemetry
{
    public class Telemetry
    {
        public TelemetryPublisher Publisher { get; set; }

        /// <summary>
        /// Class used together with the DI, holds a Publisher instance that's being create by options from
        /// TelemetryConfig.
        /// </summary>
        public Telemetry()
        {
        }

        public Telemetry(ILogger<Telemetry> logger, IOptions<TelemetryConfig> options)
        {
            if (options.Value.ServiceEnabled)
            {
                Publisher = TelemetryPublisherFactory.Create(options.Value.Publisher, options.Value.ConnectionString,
                    "NucuCar.Sensors", logger);
            }
            else
            {
                Publisher = null;
            }
        }
    }
}