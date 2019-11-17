using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NucuCar.Sensors.Telemetry
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly string _azureIotHubConnectionString;
        private readonly bool _serviceEnabled;
        private readonly int _interval;
        private readonly ILogger _logger;

        public BackgroundWorker(ILogger<BackgroundWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _serviceEnabled = configuration.GetValue<bool>("Telemetry:Enabled");
            _interval = configuration.GetValue<int>("Telemetry:Interval");
            _azureIotHubConnectionString = configuration.GetValue<string>("Telemetry:AzureIotHubConnectionString");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_serviceEnabled)
            {
                return;
            }

            await Task.Delay(_interval, stoppingToken);

            using var telemetryService = TelemetryPublisher.Instance;

            telemetryService.SetLogger(_logger);
            telemetryService.Configure(new Dictionary<string, object>()
            {
                ["AzureIotHubConnectionString"] = _azureIotHubConnectionString
            });

            telemetryService.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Publishing telemetry data!");
                await telemetryService.PublishAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}