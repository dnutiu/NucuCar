using NucuCar.Telemetry.Abstractions;

namespace NucuCar.Telemetry
{
    /// <summary>
    /// TelemetryPublisherType holds constants for instantiating <see cref="TelemetryPublisher"/>,
    /// </summary>
    public static class TelemetryPublisherType
    {
        public const string Azure = "Azure";
        public const string Disk = "Disk";
        public const string Firestore = "Firestore";
        public const string Console = "Console";
    }
}