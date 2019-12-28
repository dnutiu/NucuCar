namespace NucuCar.Domain.Telemetry
{
    /// <summary>
    /// TelemetryPublisherType holds constants for instantiating <see cref="TelemetryPublisher"/>,
    /// see <see cref="TelemetryPublisherFactory"/>.
    /// </summary>
    public static class TelemetryPublisherType
    {
        public const string Azure = "Azure";
        public const string Disk = "Disk";
    }
}