using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Telemetry
{
    public class TelemetryPublisherConsole : TelemetryPublisher
    {

        public TelemetryPublisherConsole(TelemetryPublisherBuilderOptions opts) : base(opts)
        {
        }
        
        public override Task PublishAsync(CancellationToken cancellationToken)
        {
            var data = GetTelemetry();
            var messageString = JsonConvert.SerializeObject(data);
            Logger?.LogDebug($"Telemetry message: {messageString}");
            Logger?.LogInformation(messageString);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
        }
    }
}