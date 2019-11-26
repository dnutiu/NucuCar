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
        private readonly int _measurementInterval;
        private readonly ILogger<Bme680Worker> _logger;
        private readonly TelemetryPublisher _telemetryPublisher;
        private readonly Bme680Sensor _bme680Sensor;


        public Bme680Worker(ILogger<Bme680Worker> logger, IOptions<Bme680Config> options,
            SensorTelemetry sensorTelemetry, Bme680Sensor bme680Sensor)
        {
            _logger = logger;
            _telemetryEnabled = options.Value.TelemetryEnabled;
            _measurementInterval = options.Value.MeasurementInterval;
            _telemetryPublisher = sensorTelemetry.Publisher;
            _bme680Sensor = bme680Sensor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_telemetryEnabled)
            {
                _telemetryPublisher?.RegisterTelemeter(_bme680Sensor);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                /* If sensor is ok attempt to read. */
                if (_bme680Sensor.GetState() == SensorStateEnum.Initialized)
                {
                    _logger.LogInformation("Taking measurement!");
                    await _bme680Sensor.TakeMeasurement();
                }
                /* Else attempt to re-initialize. */
                else
                {
                    await Task.Delay(10000, stoppingToken);
                    _bme680Sensor.InitializeSensor();
                }

                await Task.Delay(_measurementInterval, stoppingToken);
            }

            _telemetryPublisher?.UnRegisterTelemeter(_bme680Sensor);
        }
    }
}