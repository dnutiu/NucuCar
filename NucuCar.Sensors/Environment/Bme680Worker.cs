using Microsoft.Extensions.Logging;
using NucuCar.Sensors.Telemetry;

namespace NucuCar.Sensors.Environment
{
    public class Bme680Worker : SensorWorker
    {
        public Bme680Worker(ILogger<Bme680Worker> logger, SensorTelemetry sensorTelemetry, ISensor<Bme680Sensor> sensor)
        {
            Logger = logger;
            MeasurementInterval = 3000;
            TelemetryPublisher = sensorTelemetry.Publisher;
            Sensor = sensor.Object;
        }
    }
}