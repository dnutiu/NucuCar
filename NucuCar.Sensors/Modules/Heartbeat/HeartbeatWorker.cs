using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;
using NucuCar.Telemetry.Abstractions;

namespace NucuCar.Sensors.Modules.Heartbeat
{
    public class HeartbeatWorker : SensorWorker
    {
        public HeartbeatWorker(ILogger<HeartbeatWorker> logger, ITelemetryPublisher telemetryPublisherProxy,
            ISensor<HeartbeatSensor> sensor, IOptions<HeartbeatConfig> options)
        {
            Logger = logger;
            MeasurementInterval = options.Value.MeasurementInterval;
            TelemetryPublisher = telemetryPublisherProxy;
            Sensor = sensor.Object;
        }
    }
}