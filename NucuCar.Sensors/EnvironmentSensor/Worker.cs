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
            using var sensor = new Sensor(_logger);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (sensor.GetState() == SensorState.Initialized)
                {
                    await sensor.TakeMeasurement();
                }
                else
                {
                    /* Attempt to reinitialize the sensor. */
                    sensor.InitializeSensor();
                }
                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}