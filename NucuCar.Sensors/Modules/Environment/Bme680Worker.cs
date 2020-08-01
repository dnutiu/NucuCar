using Microsoft.Extensions.Logging;
using NucuCar.Sensors.Abstractions;

namespace NucuCar.Sensors.Modules.Environment
{
    public class Bme680Worker : SensorWorker
    {
        public Bme680Worker(ILogger<Bme680Worker> logger, Telemetry.Telemetry telemetry, ISensor<Bme680Sensor> sensor)
        {
            Logger = logger;
            MeasurementInterval = 3000;
            TelemetryPublisher = telemetry.Publisher;
            Sensor = sensor.Object;
        }
    }
}