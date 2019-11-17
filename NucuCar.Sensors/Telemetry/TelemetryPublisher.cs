using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors.Telemetry
{
    public class TelemetryPublisher : TelemetryPublisherAzure
    {
        /* Singleton Instance */
        public static TelemetryPublisher Instance { get; } = new TelemetryPublisher();

        static TelemetryPublisher()
        {
        }
    }
}