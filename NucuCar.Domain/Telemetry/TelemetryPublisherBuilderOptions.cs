using Microsoft.Extensions.Logging;

namespace NucuCar.Domain.Telemetry
{
    public class TelemetryPublisherBuilderOptions
    {
        public string ConnectionString { get; set; }
        public string TelemetrySource { get; set; }
        public ILogger Logger { get; set; }
    }
}