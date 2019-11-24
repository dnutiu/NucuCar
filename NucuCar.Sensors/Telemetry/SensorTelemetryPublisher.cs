using System;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors.Telemetry
{
    public class SensorTelemetryPublisher : IDisposable
    {
        private static object _palock = new object();
        public static TelemetryPublisher Instance { get; private set; }

        /// <summary>
        /// Creates a telemetry publisher instance see <see cref="TelemetryPublisher"/>.
        /// </summary>
        public static TelemetryPublisher CreateSingleton(string connectionString, string telemetrySource,
            ILogger logger)
        {
            lock (_palock)
            {
                if (Instance != null) return Instance;
                var telemetryPublisher =
                    TelemetryPublisherAzure.CreateFromConnectionString(connectionString, telemetrySource, logger);
                Instance = telemetryPublisher;
                return Instance;
            }
        }

        private static void ReleaseUnmanagedResources()
        {
            Instance?.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~SensorTelemetryPublisher()
        {
            ReleaseUnmanagedResources();
        }
    }
}