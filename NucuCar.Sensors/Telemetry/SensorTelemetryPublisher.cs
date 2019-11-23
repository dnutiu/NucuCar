using System;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors.Telemetry
{
    public class SensorTelemetryPublisher : IDisposable
    {
        private static object _palock = new object();
        public static TelemetryPublisher Instance { get; private set; }

        public static TelemetryPublisher CreateSingleton(string connectionString, string telemetrySource,
            ILogger logger)
        {
            if (Instance == null)
            {
                lock (_palock)
                {
                    var telemetryPublisher =
                        TelemetryPublisherAzure.CreateFromConnectionString(connectionString, telemetrySource, logger);
                    Instance = telemetryPublisher;
                }
            }

            return Instance;
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