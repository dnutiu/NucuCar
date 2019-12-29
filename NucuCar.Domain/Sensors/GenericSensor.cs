using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NucuCarSensorsProto;

namespace NucuCar.Domain.Sensors
{
    /// <summary>
    /// The GenericSensor is an abstract class, which provides a base for abstracting hardware sensors.
    /// </summary>
    public abstract class GenericSensor
    {
        protected bool TelemetryEnabled;
        protected ILogger Logger;
        protected SensorStateEnum CurrentState;

        public abstract void Initialize();
        public abstract Task TakeMeasurementAsync();
        public abstract NucuCarSensorResponse GetMeasurement();
        public abstract SensorStateEnum GetState();
    }
}