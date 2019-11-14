using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NucuCar.Sensors.Telemetry;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly bool _serviceEnabled;
        private readonly bool _telemetryEnabled;
        private readonly int _measurementDelay;
        private readonly ILogger<BackgroundWorker> _logger;


        public BackgroundWorker(ILogger<BackgroundWorker> logger, IConfiguration config)
        {
            _logger = logger;
            var configSection = config.GetSection("EnvironmentSensor");
            _serviceEnabled = configSection.GetValue<bool>("Enabled");
            _telemetryEnabled = configSection.GetValue<bool>("Telemetry");
            _measurementDelay = configSection.GetValue<int>("MeasurementInterval");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_serviceEnabled)
            {
                return;
            }

            using var sensor = Sensor.Instance;
            sensor.SetLogger(_logger);
            sensor.InitializeSensor();
            if (_telemetryEnabled)
            {
                TelemetryService.Instance.RegisterSensor(sensor);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (sensor.GetState() == SensorStateEnum.Initialized)
                {
                    await sensor.TakeMeasurement();
                }
                else
                {
                    await Task.Delay(10000, stoppingToken);
                    /* Attempt to reinitialize the sensor. */
                    sensor.InitializeSensor();
                }

                await Task.Delay(_measurementDelay, stoppingToken);
            }
            
            TelemetryService.Instance.UnregisterSensor(sensor);
        }
    }
}