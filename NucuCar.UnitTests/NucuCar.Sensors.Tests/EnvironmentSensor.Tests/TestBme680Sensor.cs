using System.Collections.Generic;
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
        
        public override Dictionary<string, double> GetMeasurement()
        {
            return new Dictionary<string, double>();
        }
        
        public override SensorStateEnum GetState()
        {
            return SensorStateEnum.Error;
        }
    }
}