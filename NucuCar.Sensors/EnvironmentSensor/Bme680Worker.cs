using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Telemetry;
using NucuCar.Sensors.Telemetry;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.EnvironmentSensor
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