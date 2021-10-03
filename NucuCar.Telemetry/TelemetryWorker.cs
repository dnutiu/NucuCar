using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Telemetry.Abstractions;

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
        private readonly ITelemetryPublisher _telemetryPublisher;

        public TelemetryWorker(ILogger<TelemetryWorker> logger, IOptions<Config> options,
            ITelemetryPublisher telemetryPublisherProxy)
        {
            _logger = logger;
            _interval = options.Value.PublishInterval;
            _serviceEnabled = options.Value.ServiceEnabled;
            _telemetryPublisher = telemetryPublisherProxy;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
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
            catch (TaskCanceledException)
            {
                _logger?.LogInformation("The TelemetryWorker task was canceled");
            }
            catch (Exception e)
            {
                _logger?.LogError("Unhandled exception in TelemetryWorker. {Message}",e.Message);
                _logger?.LogDebug("{StackTrace}", e.StackTrace);
            }
        }
    }
}