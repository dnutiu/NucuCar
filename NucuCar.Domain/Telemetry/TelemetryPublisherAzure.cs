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
    public class TelemetryPublisherAzure : TelemetryPublisher
    {
        protected readonly DeviceClient DeviceClient;

        public TelemetryPublisherAzure(TelemetryPublisherBuilderOptions opts) : base(opts)
        {
            try
            {
                DeviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);
            }
            catch (FormatException)
            {
                Logger?.LogCritical("Can't start telemetry service! Malformed connection string!");
                throw;
            }

            Logger?.LogDebug("Started the AzureTelemetryPublisher!");
        }

        /// <summary>
        /// Creates an instance of <see cref="TelemetryPublisher"/> that is used to publish data to Microsoft Azure.
        /// </summary>
        /// <param name="connectionString">The device connection string for Microsoft Azure IoT hub device.</param>
        /// <returns>A <see cref="TelemetryPublisher"/> instance.</returns>
        public static TelemetryPublisher CreateFromConnectionString(string connectionString)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            return new TelemetryPublisherAzure(new TelemetryPublisherBuilderOptions()
                {ConnectionString = connectionString, TelemetrySource = "TelemetryPublisherAzure"});
        }

        /// <summary>
        /// Creates an instance of <see cref="TelemetryPublisher"/> that is used to publish data to Microsoft Azure.
        /// </summary>
        /// <param name="connectionString">Device connection string for Microsoft Azure IoT hub device.</param>
        /// <param name="telemetrySource">String that is used to identify the source of the telemetry data.</param>
        /// <returns>A <see cref="TelemetryPublisher"/> instance.</returns>
        public static TelemetryPublisher CreateFromConnectionString(string connectionString,
            string telemetrySource)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            Guard.ArgumentNotNullOrWhiteSpace(nameof(telemetrySource), telemetrySource);
            return new TelemetryPublisherAzure(new TelemetryPublisherBuilderOptions()
                {ConnectionString = connectionString, TelemetrySource = telemetrySource});
        }

        /// <summary>
        /// Creates an instance of <see cref="TelemetryPublisher"/> that is used to publish data to Microsoft Azure.
        /// </summary>
        /// <param name="connectionString">Device connection string for Microsoft Azure IoT hub device.</param>
        /// <param name="telemetrySource">String that is used to identify the source of the telemetry data.</param>
        /// <param name="logger">An <see cref="ILogger"/> logger instance. </param>
        /// <returns>A <see cref="TelemetryPublisher"/> instance.</returns>
        public static TelemetryPublisher CreateFromConnectionString(string connectionString,
            string telemetrySource, ILogger logger)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            Guard.ArgumentNotNullOrWhiteSpace(nameof(telemetrySource), telemetrySource);
            Guard.ArgumentNotNull(nameof(logger), logger);
            return new TelemetryPublisherAzure(new TelemetryPublisherBuilderOptions()
                {ConnectionString = connectionString, TelemetrySource = telemetrySource, Logger = logger});
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            foreach (var telemeter in RegisteredTelemeters)
            {
                var data = telemeter.GetTelemetryData();
                if (data == null)
                {
                    Logger?.LogWarning($"Warning! Data for {telemeter.GetIdentifier()} is null!");
                    continue;
                }

                var metadata = new Dictionary<string, object>
                {
                    ["source"] = TelemetrySource ?? nameof(TelemetryPublisherAzure),
                    ["id"] = telemeter.GetIdentifier(),
                    ["timestamp"] = DateTime.Now,
                    ["data"] = data,
                };

                await PublishViaMqtt(metadata, cancellationToken);
            }
        }

        private async Task PublishViaMqtt(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Logger?.LogInformation("Stopping the AzureTelemetryPublisher, cancellation requested.");
                await DeviceClient.CloseAsync(cancellationToken);
                return;
            }

            var messageString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            Logger?.LogDebug($"Telemetry message: {messageString}");
            await DeviceClient.SendEventAsync(message, cancellationToken);
        }

        public override void Dispose()
        {
            DeviceClient?.CloseAsync().GetAwaiter().GetResult();
        }
    }
}