using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;

namespace NucuCar.Sensors.Modules.CpuTemperature
{
    public class CpuTempWorker : SensorWorker
    {
        public CpuTempWorker(ILogger<CpuTempWorker> logger, Telemetry.Telemetry telemetry,
            ISensor<CpuTempSensor> sensor, IOptions<CpuTempConfig> options)
        {
            Logger = logger;
            MeasurementInterval = options.Value.MeasurementInterval;
            TelemetryPublisher = telemetry.Publisher;
            Sensor = sensor.Object;
        }
    }
}