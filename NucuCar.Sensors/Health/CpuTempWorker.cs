using Microsoft.Extensions.Logging;
using NucuCar.Telemetry;

namespace NucuCar.Sensors.Health
{
    public class CpuTempWorker : SensorWorker
    {
        public CpuTempWorker(ILogger<CpuTempWorker> logger, SensorTelemetry sensorTelemetry, ISensor<CpuTempSensor> sensor)
        {
            Logger = logger;
            MeasurementInterval = 3000;
            TelemetryPublisher = sensorTelemetry.Publisher;
            Sensor = sensor.Object;
        }
    }
}