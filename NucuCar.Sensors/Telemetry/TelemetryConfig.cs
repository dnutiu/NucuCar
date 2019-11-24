using Microsoft.Extensions.Configuration;

namespace NucuCar.Sensors.Telemetry
{
    public class TelemetryConfig
    {
        public bool ServiceEnabled { get; }
        public int PublishInterval { get; }
        public string ConnectionString { get; }
        
        public TelemetryConfig(IConfiguration configuration)
        {
            ServiceEnabled = configuration.GetValue<bool>("Telemetry:Enabled");
            PublishInterval = configuration.GetValue<int>("Telemetry:PublishInterval");
            ConnectionString = configuration.GetValue<string>("Telemetry:ConnectionString");
        }
        
    }
}