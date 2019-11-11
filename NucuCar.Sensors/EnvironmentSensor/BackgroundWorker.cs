using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NucuCarGrpcSensors;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly ILogger<BackgroundWorker> _logger;


        public BackgroundWorker(ILogger<BackgroundWorker> logger, IConfiguration config)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var sensor = Sensor.Instance;
            sensor.SetLogger(_logger);
            sensor.InitializeSensor();

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

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}