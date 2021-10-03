using System;
using Microsoft.Extensions.Logging;
using NucuCar.Core.Utilities;
using NucuCar.Telemetry.Abstractions;
using NucuCar.Telemetry.Publishers;
using Console = NucuCar.Telemetry.Publishers.Console;

namespace NucuCar.Telemetry
{
    /// <summary>
    /// The PublisherFactory is used instantiate TelemetryPublishers.
    /// </summary>
    public static class PublisherFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="BasePublisher"/>. See <see cref="PublisherType"/>
        /// </summary>
        /// <param name="type">The type of the publisher. <see cref="PublisherType"/> </param>
        /// <param name="connectionString">Device connection string for the telemetry publisher.</param>
        /// <param name="telemetrySource">String that is used to identify the source of the telemetry data.</param>
        /// <param name="logger">An <see cref="ILogger"/> logger instance. </param>
        /// <returns>A <see cref="BasePublisher"/> instance.</returns>
        public static ITelemetryPublisher Create(string type, string connectionString,
            string telemetrySource, ILogger logger)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            Guard.ArgumentNotNullOrWhiteSpace(nameof(telemetrySource), telemetrySource);
            Guard.ArgumentNotNull(nameof(logger), logger);
            var opts = new PublisherOptions
                {ConnectionString = connectionString, TelemetrySource = telemetrySource, Logger = logger};
            return SpawnPublisher(type, opts);
        }

        /// <summary>
        /// Creates an instance of <see cref="BasePublisher"/>.
        /// </summary>
        /// <param name="type">The type of the publisher. See <see cref="PublisherType"/> </param>
        /// <param name="connectionString">The device connection string for the selected publisher.</param>
        /// <returns>A <see cref="BasePublisher"/> instance.</returns>
        public static ITelemetryPublisher CreateFromConnectionString(string type, string connectionString)
        {
            Guard.ArgumentNotNullOrWhiteSpace(nameof(connectionString), connectionString);
            var opts = new PublisherOptions()
                {ConnectionString = connectionString, TelemetrySource = "NucuCar.Sensors"};
            return SpawnPublisher(type, opts);
        }

        private static ITelemetryPublisher SpawnPublisher(string type, PublisherOptions opts)
        {
            return type switch
            {
                PublisherType.Azure => new Azure(opts),
                PublisherType.Disk => new Disk(opts),
                PublisherType.Console => new Console(opts),
                _ => throw new ArgumentException($"Invalid TelemetryPublisher type: {type}.")
            };
        }
    }
}