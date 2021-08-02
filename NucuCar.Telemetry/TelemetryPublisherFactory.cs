using System;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Utilities;
using NucuCar.Telemetry.Abstractions;
using NucuCar.Telemetry.Publishers;

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
        /// <param name="connectionString">Device connection string for the telemetry publisher.</param>
        /// <param name="telemetrySource">String that is used to identify the source of the telemetry data.</param>
        /// <param name="logger">An <see cref="ILogger"/> logger instance. </param>
        /// <returns>A <see cref="TelemetryPublisher"/> instance.</returns>
        public static ITelemetryPublisher Create(string type, string connectionString,
            string telemetrySource, ILogger logger)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            Guard.ArgumentNotNullOrWhiteSpace(nameof(telemetrySource), telemetrySource);
            Guard.ArgumentNotNull(nameof(logger), logger);
            var opts = new TelemetryPublisherOptions
                {ConnectionString = connectionString, TelemetrySource = telemetrySource, Logger = logger};
            return SpawnPublisher(type, opts);
        }

        /// <summary>
        /// Creates an instance of <see cref="TelemetryPublisher"/>.
        /// </summary>
        /// <param name="type">The type of the publisher. See <see cref="TelemetryPublisherType"/> </param>
        /// <param name="connectionString">The device connection string for the selected publisher.</param>
        /// <returns>A <see cref="TelemetryPublisher"/> instance.</returns>
        public static ITelemetryPublisher CreateFromConnectionString(string type, string connectionString)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            var opts = new TelemetryPublisherOptions()
                {ConnectionString = connectionString, TelemetrySource = "NucuCar.Sensors"};
            return SpawnPublisher(type, opts);
        }

        private static ITelemetryPublisher SpawnPublisher(string type, TelemetryPublisherOptions opts)
        {
            return type switch
            {
                TelemetryPublisherType.Azure => new TelemetryPublisherAzure(opts),
                TelemetryPublisherType.Disk => new TelemetryPublisherDisk(opts),
                TelemetryPublisherType.Firestore => new TelemetryPublisherFirestore(opts),
                TelemetryPublisherType.Console => new TelemetryPublisherConsole(opts),
                _ => throw new ArgumentException($"Invalid TelemetryPublisher type: {type}.")
            };
        }
    }
}