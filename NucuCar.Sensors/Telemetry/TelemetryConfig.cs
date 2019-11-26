// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NucuCar.Sensors.Telemetry
{
    public class TelemetryConfig
    {
        public bool ServiceEnabled { get; set; }
        public int PublishInterval { get; set; }
        public string ConnectionString { get; set; }
    }
}