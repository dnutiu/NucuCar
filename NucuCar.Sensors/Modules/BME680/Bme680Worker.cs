using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;

namespace NucuCar.Sensors.Modules.BME680
{
    public class Bme680Worker : SensorWorker
    {
        public Bme680Worker(ILogger<Bme680Worker> logger, Telemetry.Telemetry telemetry, ISensor<Bme680Sensor> sensor,
            IOptions<Bme680Config> options)
        {
            Logger = logger;
            MeasurementInterval = options.Value.MeasurementInterval;
            TelemetryPublisher = telemetry.Publisher;
            Sensor = sensor.Object;
        }
    }
}