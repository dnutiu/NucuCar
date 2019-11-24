using Microsoft.Extensions.Configuration;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public class Bme680Config
    {
        public bool SensorEnabled { get; }
        public bool TelemetryEnabled { get; }
        public int MeasurementInterval { get; }
        
        public Bme680Config(IConfiguration configuration)
        {
            SensorEnabled = configuration.GetValue<bool>("EnvironmentSensor:Enabled");
            TelemetryEnabled = configuration.GetValue<bool>("EnvironmentSensor:TelemetryEnabled");
            MeasurementInterval = configuration.GetValue<int>("EnvironmentSensor:MeasurementInterval");
        }
    }
}