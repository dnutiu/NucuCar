using System.Threading.Tasks;
using NucuCar.Sensors.Abstractions;
using NucuCar.Sensors.Modules.Environment;

namespace NucuCar.UnitTests.NucuCar.Sensors.Tests.EnvironmentSensor
{
    public class TestBme680Sensor : Bme680Sensor
    {
        public override Task TakeMeasurementAsync()
        {
            return Task.CompletedTask;
        }
        
        public override void Initialize()
        {
            
        }
        
        public override NucuCarSensorResponse GetMeasurement()
        {
            return new NucuCarSensorResponse();
        }
        
        public override SensorStateEnum GetState()
        {
            return SensorStateEnum.Error;
        }
    }
}