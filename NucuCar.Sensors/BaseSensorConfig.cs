using System.Diagnostics.CodeAnalysis;

namespace NucuCar.Sensors
{
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class BaseSensorConfig
    {
        public bool Enabled { get; set; } = false;
        public bool Telemetry { get; set; } = false;

        public int MeasurementInterval { get; set; } = 3000;
    }
}