// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NucuCar.Sensors.EnvironmentSensor
{
    public class Bme680Config
    {
        public bool ServiceEnabled { get; set; }
        public bool TelemetryEnabled { get; set; }
        public int MeasurementInterval { get; set; }
    }
}