using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NucuCar.Domain.Telemetry
{
    public class TelemetryPublisherAzure : TelemetryPublisher, IDisposable
    {
        // Needs to be configured via the Configure method or setup directly.
        public string AzureIotHubConnectionString { get; set; }
        protected DeviceClient DeviceClient;

        public override void Start()
        {
            try
            {
                DeviceClient = DeviceClient.CreateFromConnectionString(AzureIotHubConnectionString, TransportType.Mqtt);
            }
            catch (FormatException)
            {
                Logger.LogCritical("Can't start telemetry service! Malformed connection string!");
                throw;
            }
            Logger.LogInformation("Started the AzureTelemetryPublisher!");
        }
        public override void Configure(Dictionary<string, object> config)
        {
            AzureIotHubConnectionString = config.GetValueOrDefault("AzureIotHubConnectionString").ToString();
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            foreach (var telemeter in RegisteredTelemeters)
            {
                var data = telemeter.GetTelemetryData();
                if (data == null)
                {
                    Logger.LogWarning($"Warning! Data for {telemeter.GetIdentifier()} is null!");
                    continue;
                }
                
                data["id"] = telemeter.GetIdentifier();
                data["timestamp"] = DateTime.Now;
                await PublishViaMqtt(data, cancellationToken);
            }
        }

        private async Task PublishViaMqtt(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Logger.LogInformation("Stopping the AzureTelemetryPublisher, cancellation requested.");
                await DeviceClient.CloseAsync(cancellationToken);
                return;
            }
            var messageString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            Logger.LogDebug($"Telemetry message: {message}");
            await DeviceClient.SendEventAsync(message, cancellationToken);
        }
        
        public void Dispose()
        {
            DeviceClient.CloseAsync().GetAwaiter().GetResult();
        }
        
        public override bool Publish(int timeout)
        {
            throw new NotImplementedException();
        }
        
#pragma warning disable 1998
        public override async Task StartAsync()
        {
            throw new NotImplementedException();
        }
#pragma warning restore 1998
    }
}