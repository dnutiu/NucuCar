using System.Threading.Tasks;
using NucuCar.Sensors.Abstractions;
using NucuCar.Sensors.Modules.BME680;

namespace NucuCar.UnitTests.NucuCar.Sensors.Bme680
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
        
        public override SensorResponse GetMeasurement()
        {
            return new SensorResponse();
        }
        
        public override SensorStateEnum GetState()
        {
            return SensorStateEnum.Error;
        }
    }
}