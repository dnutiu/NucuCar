using NucuCar.Sensors.EnvironmentSensor;
using NucuCarSensorsProto;

namespace NucuCar.UnitTests.NucuCar.Sensors.Tests.EnvironmentSensor.Tests
{
    public partial class Bme680GrpcServiceTest
    {
        public class TestBme680Sensor : Bme680Sensor
        {
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
}