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
        protected SensorStateEnum SensorStateEnum;

        public abstract void Initialize();
        public abstract Task TakeMeasurementAsync();
        public abstract Bme680MeasurementData GetMeasurement();
        public abstract SensorStateEnum GetState();
    }
}