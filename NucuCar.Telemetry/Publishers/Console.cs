using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NucuCar.Telemetry.Publishers
{
    public class Console : BasePublisher
    {

        public Console(PublisherOptions opts) : base(opts)
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