using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NucuCar.Telemetry.Publishers
{
    /// <summary>
    /// Constructs an instance of <see cref="Azure"/>. It is used to publish telemetry to Microsoft
    /// Azure IotHub
    /// <remarks>
    ///    The connection string can be found in your Azure IotHub.
    /// </remarks>
    /// </summary>
    public class Azure : BasePublisher
    {
        protected readonly DeviceClient DeviceClient;

        public Azure(PublisherOptions opts) : base(opts)
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
            Logger?.LogDebug("Telemetry message: {Message}", messageString);
            var message = new Message(Encoding.UTF8.GetBytes(messageString));

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
                    Logger?.LogWarning("Telemetry not sent! Retry attempt #{Retry}", retry);
                }
            }
        }

        public override void Dispose()
        {
            DeviceClient?.CloseAsync().GetAwaiter().GetResult();
        }
    }
}