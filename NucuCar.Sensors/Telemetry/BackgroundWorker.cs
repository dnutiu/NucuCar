using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NucuCar.Sensors.Telemetry
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly bool _serviceEnabled;
        private readonly int _interval;
        private readonly ILogger _logger;

        public BackgroundWorker(ILogger<BackgroundWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            var configSection = configuration.GetSection("Telemetry");
            _serviceEnabled = configSection.GetValue<bool>("Enabled");
            _interval = configSection.GetValue<int>("Interval");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_serviceEnabled)
            {
                return;
            }
            await Task.Delay(_interval, stoppingToken);

            using var telemetryService = TelemetryService.Instance;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Publishing telemetry data!");
                await telemetryService.PublishData();
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}