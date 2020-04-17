// ReSharper disable UnusedAutoPropertyAccessor.Global

using NucuCar.Domain.Telemetry;

namespace NucuCar.Telemetry
{
    public class TelemetryConfig
    {
        /// <summary>
        ///  The Publisher is used by <see cref="TelemetryPublisherFactory"/> to instantiate
        /// the correct <see cref="TelemetryPublisher"/>. For available types see <see cref="TelemetryPublisherType"/>
        /// </summary>
        public string Publisher { get; set; }

        public bool ServiceEnabled { get; set; }
        public int PublishInterval { get; set; }
        public string ConnectionString { get; set; }
    }
}