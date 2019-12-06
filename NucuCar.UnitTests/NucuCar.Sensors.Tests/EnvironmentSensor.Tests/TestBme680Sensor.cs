using System.Collections.Generic;
using System.Threading.Tasks;
using NucuCar.Domain.Sensors;
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
        
        public override Bme680MeasurementData GetMeasurement()
        {
            return new Bme680MeasurementData();
        }
        
        public override SensorStateEnum GetState()
        {
            return SensorStateEnum.Error;
        }
    }
}