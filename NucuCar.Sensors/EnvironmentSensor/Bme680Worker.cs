using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly bool _serviceEnabled;
        private readonly bool _telemetryEnabled;
        private readonly int _measurementDelay;
        private readonly ILogger<Bme680Worker> _logger;
        private readonly TelemetryPublisher _telemetryPublisher;
        private readonly Bme680Sensor _bme680Sensor;


        public Bme680Worker(ILogger<Bme680Worker> logger, IConfiguration config,
            SensorTelemetry sensorTelemetry, Bme680Sensor bme680Sensor)
        {
            _logger = logger;
            _serviceEnabled = config.GetValue<bool>("EnvironmentSensor:Enabled");
            _telemetryEnabled = config.GetValue<bool>("EnvironmentSensor:Telemetry");
            _measurementDelay = config.GetValue<int>("EnvironmentSensor:MeasurementInterval");
            _telemetryPublisher = sensorTelemetry.Publisher;
            _bme680Sensor = bme680Sensor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_serviceEnabled)
            {
                return;
            }
            
            if (_telemetryEnabled)
            {
                _telemetryPublisher?.RegisterTelemeter(_bme680Sensor);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                /* If sensor is ok attempt to read. */
                if (_bme680Sensor.GetState() == SensorStateEnum.Initialized)
                {
                    await _bme680Sensor.TakeMeasurement();
                }
                /* Else attempt to re-initialize. */
                else
                {
                    await Task.Delay(10000, stoppingToken);
                    _bme680Sensor.InitializeSensor();
                }

                await Task.Delay(_measurementDelay, stoppingToken);
            }

            _telemetryPublisher?.UnRegisterTelemeter(_bme680Sensor);
        }
    }
}