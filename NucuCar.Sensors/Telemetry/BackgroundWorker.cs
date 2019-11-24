using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NucuCar.Sensors.Telemetry
{
    /// <summary>
    /// Telemetry service, which pools the telemetry sources and pushes telemetry data to the cloud.
    /// </summary>
    public class BackgroundWorker : BackgroundService
    {
        private readonly bool _serviceEnabled;
        private readonly int _interval;
        private readonly ILogger _logger;

        public BackgroundWorker(ILogger<BackgroundWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _serviceEnabled = configuration.GetValue<bool>("Telemetry:Enabled");
            _interval = configuration.GetValue<int>("Telemetry:Interval");
            var connectionString = configuration.GetValue<string>("Telemetry:ConnectionString");
            if (_serviceEnabled)
            {
                SensorTelemetryPublisher.CreateSingleton(connectionString, "NucuCar.Sensors", logger);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_serviceEnabled)
            {
                return;
            }

            await Task.Delay(_interval, stoppingToken);

            var telemetryService = SensorTelemetryPublisher.Instance;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Publishing telemetry data!");
                await telemetryService.PublishAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            SensorTelemetryPublisher.Instance?.Dispose();
        }
    }
}