using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NucuCar.Sensors.Abstractions
{
    /// <summary>
    /// The GenericSensor is an abstract class, which provides a base for abstracting hardware sensors.
    /// </summary>
    public abstract class GenericSensor
    {
        protected ILogger Logger;
        protected SensorStateEnum CurrentState;

        public abstract void Initialize();
        public abstract Task TakeMeasurementAsync();
        public abstract SensorResponse GetMeasurement();
        public abstract SensorStateEnum GetState();
    }
}