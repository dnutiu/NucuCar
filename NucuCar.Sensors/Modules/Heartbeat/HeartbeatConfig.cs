namespace NucuCar.Sensors.Modules.Heartbeat
{
    public class HeartbeatConfig
    {
        public bool Enabled { get; set; }
        public bool Telemetry { get; set; }
        public bool Grpc { get; } = false;
    }
}