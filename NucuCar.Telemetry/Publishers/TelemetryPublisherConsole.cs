using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NucuCar.Telemetry.Abstractions;

namespace NucuCar.Telemetry.Publishers
{
    public class TelemetryPublisherConsole : TelemetryPublisher
    {

        public TelemetryPublisherConsole(TelemetryPublisherOptions opts) : base(opts)
        {
        }
        
        public override Task PublishAsync(CancellationToken cancellationToken)
        {
            var data = GetTelemetry();
            var messageString = JsonConvert.SerializeObject(data);
            Logger?.LogDebug("Telemetry message: {Message}", messageString);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
        }
    }
}