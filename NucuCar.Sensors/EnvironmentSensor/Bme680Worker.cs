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
    public class BackgroundWorker : BackgroundService
    {
        private readonly bool _serviceEnabled;
        private readonly bool _telemetryEnabled;
        private readonly int _measurementDelay;
        private readonly ILogger<BackgroundWorker> _logger;
        private readonly TelemetryPublisher _telemetryPublisher;


        public BackgroundWorker(ILogger<BackgroundWorker> logger, IConfiguration config,
            SensorTelemetry sensorTelemetry)
        {
            _logger = logger;
            _serviceEnabled = config.GetValue<bool>("EnvironmentSensor:Enabled");
            _telemetryEnabled = config.GetValue<bool>("EnvironmentSensor:Telemetry");
            _measurementDelay = config.GetValue<int>("EnvironmentSensor:MeasurementInterval");
            _telemetryPublisher = sensorTelemetry.Publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_serviceEnabled)
            {
                return;
            }

            using var sensor = Bme680Sensor.Instance;
            sensor.Logger = _logger;
            sensor.InitializeSensor();
            if (_telemetryEnabled)
            {
                _telemetryPublisher?.RegisterTelemeter(sensor);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                /* If sensor is ok attempt to read. */
                if (sensor.GetState() == SensorStateEnum.Initialized)
                {
                    await sensor.TakeMeasurement();
                }
                /* Else attempt to re-initialize. */
                else
                {
                    await Task.Delay(10000, stoppingToken);
                    sensor.InitializeSensor();
                }

                await Task.Delay(_measurementDelay, stoppingToken);
            }

            _telemetryPublisher?.UnRegisterTelemeter(sensor);
        }
    }
}