using NucuCar.Telemetry.Publishers;

namespace NucuCar.Telemetry
{
    /// <summary>
    /// TelemetryPublisherType holds constants for instantiating <see cref="BasePublisher"/>,
    /// </summary>
    public static class PublisherType
    {
        public const string Azure = "Azure";
        public const string Disk = "Disk";
        public const string Firestore = "Firestore";
        public const string Console = "Console";
    }
}