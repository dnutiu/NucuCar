using System.Threading.Tasks;
using NucuCar.Sensors.EnvironmentSensor;
using NucuCarSensorsProto;

namespace NucuCar.UnitTests.NucuCar.Sensors.Tests.EnvironmentSensor.Tests
{
    public class TestBme680Sensor : Bme680Sensor
    {
        public override Task TakeMeasurement()
        {
            return Task.CompletedTask;
        }
        
        public override void InitializeSensor()
        {
            
        }
        
        // TODO Make more generic
        public override EnvironmentSensorMeasurement GetMeasurement()
        {
            return new EnvironmentSensorMeasurement();
        }
        
        public override SensorStateEnum GetState()
        {
            return SensorStateEnum.Error;
        }
    }
}