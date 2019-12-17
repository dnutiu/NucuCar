using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NucuCarSensorsProto;

namespace NucuCar.Domain.Sensors
{
    public abstract class GenericSensor
    {
        protected bool TelemetryEnabled;
        protected ILogger Logger;
        protected SensorStateEnum SensorStateEnum;

        public abstract void InitializeSensor();
        public abstract Task TakeMeasurementAsync();
        public abstract Bme680MeasurementData GetMeasurement();
        public abstract SensorStateEnum GetState();
    }
}