using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;
using NucuCar.Telemetry.Abstractions;

namespace NucuCar.Sensors.Modules.CpuTemperature
{
    public class CpuTempWorker : SensorWorker
    {
        public CpuTempWorker(ILogger<CpuTempWorker> logger, ITelemetryPublisher telemetryPublisherProxy,
            ISensor<CpuTempSensor> sensor, IOptions<CpuTempConfig> options)
        {
            Logger = logger;
            MeasurementInterval = options.Value.MeasurementInterval;
            TelemetryPublisher = telemetryPublisherProxy;
            Sensor = sensor.Object;
        }
    }
}