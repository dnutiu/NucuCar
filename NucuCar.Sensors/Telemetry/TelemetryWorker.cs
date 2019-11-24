using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors.Telemetry
{
    /// <summary>
    /// Telemetry service, which pools the telemetry sources and pushes telemetry data to the cloud.
    /// </summary>
    public class TelemetryWorker : BackgroundService
    {
        private readonly int _interval;
        private readonly ILogger _logger;
        private readonly TelemetryPublisher _telemetryPublisher;

        public TelemetryWorker(ILogger<TelemetryWorker> logger, TelemetryConfig config,
            SensorTelemetry sensorTelemetry)
        {
            _logger = logger;
            _interval = config.PublishInterval;
            _telemetryPublisher = sensorTelemetry.Publisher;
            if (_telemetryPublisher == null)
            {
                logger.LogCritical("Invalid state! TelemetryPublisher is null!");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(_interval, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Is publishing telemetry data!");
                await _telemetryPublisher.PublishAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}