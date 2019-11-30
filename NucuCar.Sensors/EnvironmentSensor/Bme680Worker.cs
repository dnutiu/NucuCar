using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Domain.Telemetry;
using NucuCar.Sensors.Telemetry;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.EnvironmentSensor
{
    /// <summary>
    /// EnvironmentSensor's background service worker.
    /// It does periodic reads from the sensors and publishes telemetry data if the option is enabled.
    /// </summary>
    public class Bme680Worker : BackgroundService
    {
        private readonly bool _telemetryEnabled;
        private readonly bool _serviceEnabled;
        private readonly int _measurementInterval;
        private readonly ILogger<Bme680Worker> _logger;
        private readonly TelemetryPublisher _telemetryPublisher;
        private readonly ISensor<Bme680Sensor> _bme680Sensor;


        public Bme680Worker(ILogger<Bme680Worker> logger, IOptions<Bme680Config> options,
            SensorTelemetry sensorTelemetry, ISensor<Bme680Sensor> sensor)
        {
            _logger = logger;
            _telemetryEnabled = options.Value.TelemetryEnabled;
            _serviceEnabled = options.Value.ServiceEnabled;
            _measurementInterval = options.Value.MeasurementInterval;
            _telemetryPublisher = sensorTelemetry.Publisher;
            _bme680Sensor = sensor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_serviceEnabled)
            {
                return;
            }
            if (_telemetryEnabled)
            {
                _telemetryPublisher?.RegisterTelemeter(_bme680Sensor.Object);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                /* If sensor is ok attempt to read. */
                if (_bme680Sensor.Object.GetState() == SensorStateEnum.Initialized)
                {
                    _logger.LogInformation("Taking measurement!");
                    await _bme680Sensor.Object.TakeMeasurement();
                }
                /* Else attempt to re-initialize. */
                else
                {
                    await Task.Delay(10000, stoppingToken);
                    _bme680Sensor.Object.InitializeSensor();
                }

                await Task.Delay(_measurementInterval, stoppingToken);
            }

            _telemetryPublisher?.UnRegisterTelemeter(_bme680Sensor.Object);
        }
    }
}