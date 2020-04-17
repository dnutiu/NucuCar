using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Telemetry
{
    /// <summary>
    /// Telemetry service, which pools the telemetry sources and pushes telemetry data to the cloud.
    /// </summary>
    public class TelemetryWorker : BackgroundService
    {
        private readonly int _interval;
        private readonly bool _serviceEnabled;
        private readonly ILogger _logger;
        private readonly TelemetryPublisher _telemetryPublisher;

        public TelemetryWorker(ILogger<TelemetryWorker> logger, IOptions<TelemetryConfig> options,
            SensorTelemetry sensorTelemetry)
        {
            _logger = logger;
            _interval = options.Value.PublishInterval;
            _serviceEnabled = options.Value.ServiceEnabled;
            _telemetryPublisher = sensorTelemetry.Publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_serviceEnabled)
            {
                _logger.LogInformation("Telemetry service is disabled!");
                return;
            }

            if (_telemetryPublisher == null)
            {
                _logger?.LogCritical("Invalid state! TelemetryPublisher is null!");
                return;
            }

            await Task.Delay(_interval, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await _telemetryPublisher.PublishAsync(stoppingToken);
                _logger?.LogDebug("Telemetry data published!");
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}