using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;
using NucuCar.Telemetry.Abstractions;

namespace NucuCar.Sensors.Modules.BME680
{
    public class Bme680Worker : SensorWorker
    {
        public Bme680Worker(ILogger<Bme680Worker> logger, ITelemetryPublisher telemetryPublisherProxy, ISensor<Bme680Sensor> sensor,
            IOptions<Bme680Config> options)
        {
            Logger = logger;
            MeasurementInterval = options.Value.MeasurementInterval;
            TelemetryPublisher = telemetryPublisherProxy;
            Sensor = sensor.Object;
        }
    }
}