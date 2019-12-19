using System.Threading.Tasks;
using NucuCar.Sensors.EnvironmentSensor;
using NucuCarSensorsProto;

namespace NucuCar.UnitTests.NucuCar.Sensors.Tests.EnvironmentSensor.Tests
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