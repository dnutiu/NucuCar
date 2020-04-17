using System;
using Microsoft.Extensions.Logging;
using NucuCar.Domain;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Telemetry
{
    /// <summary>
    /// The TelemetryPublisherFactory is used instantiate TelemetryPublishers.
    /// </summary>
    public static class TelemetryPublisherFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="TelemetryPublisher"/>. See <see cref="TelemetryPublisherType"/>
        /// </summary>
        /// <param name="type">The type of the publisher. <see cref="TelemetryPublisherType"/> </param>
        /// <param name="connectionString">Device connection string for Microsoft Azure IoT hub device.</param>
        /// <param name="telemetrySource">String that is used to identify the source of the telemetry data.</param>
        /// <param name="logger">An <see cref="ILogger"/> logger instance. </param>
        /// <returns>A <see cref="TelemetryPublisher"/> instance.</returns>
        public static TelemetryPublisher Create(string type, string connectionString,
            string telemetrySource, ILogger logger)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            Guard.ArgumentNotNullOrWhiteSpace(nameof(telemetrySource), telemetrySource);
            Guard.ArgumentNotNull(nameof(logger), logger);
            var opts = new TelemetryPublisherBuilderOptions()
                {ConnectionString = connectionString, TelemetrySource = telemetrySource, Logger = logger};
            return SpawnPublisher(type, opts);
        }

        /// <summary>
        /// Creates an instance of <see cref="TelemetryPublisher"/>.
        /// </summary>
        /// <param name="type">The type of the publisher. See <see cref="TelemetryPublisherType"/> </param>
        /// <param name="connectionString">The device connection string for the selected publisher.</param>
        /// <returns>A <see cref="TelemetryPublisher"/> instance.</returns>
        public static TelemetryPublisher CreateFromConnectionString(string type, string connectionString)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            var opts = new TelemetryPublisherBuilderOptions()
                {ConnectionString = connectionString, TelemetrySource = "TelemetryPublisherAzure"};
            return SpawnPublisher(type, opts);
        }

        private static TelemetryPublisher SpawnPublisher(string type, TelemetryPublisherBuilderOptions opts)
        {
            return type switch
            {
                TelemetryPublisherType.Azure => (TelemetryPublisher) new TelemetryPublisherAzure(opts),
                TelemetryPublisherType.Disk => new TelemetryPublisherDisk(opts),
                TelemetryPublisherType.Firestore => new TelemetryPublisherFirestore(opts),
                _ => throw new ArgumentException($"Invalid TelemetryPublisher type: {type}.")
            };
        }
    }
}