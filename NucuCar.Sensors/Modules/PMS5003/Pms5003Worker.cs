using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;

namespace NucuCar.Sensors.Modules.PMS5003
{
    public class Pms5003Worker : SensorWorker
    {
        public Pms5003Worker(ILogger<Pms5003Worker> logger, Telemetry.Telemetry telemetry,
            ISensor<Pms5003Sensor> sensor, IOptions<Pms5003Config> options)
        {
            Logger = logger;
            MeasurementInterval = options.Value.MeasurementInterval;
            TelemetryPublisher = telemetry.Publisher;
            Sensor = sensor.Object;
        }
    }
}