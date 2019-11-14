using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NucuCar.Sensors.Telemetry
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly string _projectId;
        private readonly string _region;
        private readonly string _registryId;
        private readonly string _deviceId;
        private readonly string _rs256KeyFile;
        private readonly bool _serviceEnabled;
        private readonly int _interval;
        private readonly ILogger _logger;

        public BackgroundWorker(ILogger<BackgroundWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            var configSection = configuration.GetSection("Telemetry");
            _serviceEnabled = configSection.GetValue<bool>("Enabled");
            _interval = configSection.GetValue<int>("Interval");
            _projectId = configSection.GetValue<string>("ProjectId");
            _region = configSection.GetValue<string>("Region");
            _registryId = configSection.GetValue<string>("RegistryId");
            _deviceId = configSection.GetValue<string>("DeviceId");
            _rs256KeyFile = configSection.GetValue<string>("RS256File");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_serviceEnabled)
            {
                return;
            }

            await Task.Delay(_interval, stoppingToken);

            using var telemetryService = TelemetryService.Instance;

            telemetryService.SetLogger(_logger);
            telemetryService.ProjectId = _projectId;
            telemetryService.DeviceId = _deviceId;
            telemetryService.RegistryId = _registryId;
            telemetryService.Region = _region;
            telemetryService.Rs256File = _rs256KeyFile;

            await telemetryService.StartAsync();
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Publishing telemetry data!");
                await telemetryService.PublishDataAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}