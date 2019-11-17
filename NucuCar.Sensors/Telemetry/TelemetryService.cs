using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NucuCar.Sensors.Telemetry
{
    public class TelemetryService : IDisposable
    {
        private readonly List<ITelemetrySensor> _registeredSensors;
        private DeviceClient _deviceClient;
        private ILogger _logger;

        /* Singleton Instance */
        public static TelemetryService Instance { get; } = new TelemetryService();
        public string AzureIotHubConnectionString { get; set; }

        static TelemetryService()
        {
        }


        private TelemetryService()
        {
            _registeredSensors = new List<ITelemetrySensor>(5);
        }

        public void Dispose()
        {
        }

        public void Start()
        {
            try
            {
                _deviceClient = DeviceClient.CreateFromConnectionString(AzureIotHubConnectionString, TransportType.Mqtt);
            }
            catch (FormatException)
            {
                _logger.LogCritical("Can't start telemetry service! Malformed connection string!");
                throw;
            }
            _logger.LogInformation("Started the MQTT client!");
        }

        public async Task PublishDataAsync(CancellationToken cancellationToken)
        {
            foreach (var sensor in _registeredSensors)
            {
                var data = sensor.GetTelemetryData();
                if (data == null)
                {
                    _logger.LogWarning($"Warning! Data for {sensor.GetIdentifier()} is null!");
                    continue;
                }

                await UploadData(data, cancellationToken);
            }
        }

        private async Task UploadData(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Stopping the MQTT client, cancellation requested.");
                await _deviceClient.CloseAsync(cancellationToken);
                return;
            }
            var messageString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            _logger.LogDebug($"Telemetry message: {message}");
            await _deviceClient.SendEventAsync(message, cancellationToken);
        }

        public void RegisterSensor(ITelemetrySensor sensor)
        {
            _registeredSensors.Add(sensor);
        }

        public void UnregisterSensor(ITelemetrySensor sensor)
        {
            _registeredSensors.Remove(sensor);
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }
    }
}