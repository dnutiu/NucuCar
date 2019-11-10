using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;


        public Worker(ILogger<Worker> logger)
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
                if (sensor.GetState() == SensorState.Initialized)
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