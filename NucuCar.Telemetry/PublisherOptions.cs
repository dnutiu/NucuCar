using Microsoft.Extensions.Logging;
using NucuCar.Telemetry.Publishers;

namespace NucuCar.Telemetry
{
    /// <summary>
    /// This class contains options for the <see cref="BasePublisher"/>.
    /// </summary>
    public class PublisherOptions
    {
        /// <summary>
        ///  The ConnectionString used by the publisher to connect to the cloud service.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// A string that indicates the source of the telemetry data.
        /// </summary>
        public string TelemetrySource { get; set; }

        /// <summary>
        /// The <see cref="ILogger"/> logger instance.
        /// </summary>
        public ILogger Logger { get; set; }
    }
}