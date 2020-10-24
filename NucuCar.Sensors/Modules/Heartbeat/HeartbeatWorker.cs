using Microsoft.Extensions.Logging;
using NucuCar.Sensors.Abstractions;

namespace NucuCar.Sensors.Modules.Heartbeat
{
    public class HeartbeatWorker: SensorWorker
    {
        public HeartbeatWorker(ILogger<HeartbeatWorker> logger, Telemetry.Telemetry telemetry, ISensor<HeartbeatSensor> sensor)
            {
                Logger = logger;
                MeasurementInterval = 3000;
                TelemetryPublisher = telemetry.Publisher;
                Sensor = sensor.Object;
            }
    }
}