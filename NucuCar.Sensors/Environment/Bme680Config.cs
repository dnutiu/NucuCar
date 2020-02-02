// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NucuCar.Sensors.Environment
{
    public class Bme680Config
    {
        public bool Enabled { get; set; }
        public bool Telemetry { get; set; }
        public bool Grpc { get; set; }
    }
}