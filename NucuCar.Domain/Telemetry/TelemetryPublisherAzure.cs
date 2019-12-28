using System;
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

            Logger?.LogDebug("Initialized the AzureTelemetryPublisher!");
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            var data = GetTelemetry();

            var messageString = JsonConvert.SerializeObject(data);
            Logger?.LogDebug($"Telemetry message: {messageString}");
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            await PublishToCloudAsync(message, cancellationToken);
        }

        private async Task PublishToCloudAsync(Message message, CancellationToken cancellationToken, int maxRetries = 3)
        {
            var retry = 0;
            while (retry < maxRetries)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Logger?.LogInformation("Publishing telemetry cancelled!");
                    break;
                }

                try
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(5000);
                    cts.Token.ThrowIfCancellationRequested();

                    /* Should throw OperationCanceledException on timeout or cancel. */
                    await DeviceClient.SendEventAsync(message, cts.Token);
                    break;
                }
                catch (OperationCanceledException)
                {
                    retry += 1;
                    Logger?.LogWarning($"Telemetry not sent! Retry {retry}.");
                }
            }
        }

        public override void Dispose()
        {
            DeviceClient?.CloseAsync().GetAwaiter().GetResult();
        }
    }
}