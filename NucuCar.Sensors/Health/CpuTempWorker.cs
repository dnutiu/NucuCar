using Microsoft.Extensions.Logging;
using NucuCar.Sensors.Abstractions;
using NucuCar.Telemetry;

namespace NucuCar.Sensors.Health
{
    public class CpuTempWorker : SensorWorker
    {
        public CpuTempWorker(ILogger<CpuTempWorker> logger, Telemetry.Telemetry telemetry, ISensor<CpuTempSensor> sensor)
        {
            Logger = logger;
            MeasurementInterval = 3000;
            TelemetryPublisher = telemetry.Publisher;
            Sensor = sensor.Object;
        }
    }
}