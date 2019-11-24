using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors.Telemetry
{
    /// <summary>
    /// Telemetry service, which pools the telemetry sources and pushes telemetry data to the cloud.
    /// </summary>
    public class TelemetryBackgroundWorker : BackgroundService
    {
        private readonly int _interval;
        private readonly ILogger _logger;
        private readonly SensorTelemetry _sensorTelemetry;

        public TelemetryBackgroundWorker(ILogger<TelemetryBackgroundWorker> logger, IConfiguration configuration,
            SensorTelemetry sensorTelemetry)
        {
            _logger = logger;
            _interval = configuration.GetValue<int>("Telemetry:Interval");
            _sensorTelemetry = sensorTelemetry;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(_interval, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Is publishing telemetry data!");
                await _sensorTelemetry.Publisher.PublishAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}