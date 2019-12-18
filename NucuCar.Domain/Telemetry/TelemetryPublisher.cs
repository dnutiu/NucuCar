using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NucuCar.Domain.Telemetry
{
    /// <summary>
    /// The TelemetryPublisher is an abstract class, which provides a base for implementing telemetry publishers.
    /// </summary>
    public abstract class TelemetryPublisher : IDisposable
    {
        /// <summary>
        /// Raw connection string that is used to connect to the cloud service. Should be parsed if required.
        /// </summary>
        protected string ConnectionString { get; set; }

        /// <summary>
        /// Telemetry source that indicates the source of the telemetry data.
        /// </summary>
        protected string TelemetrySource { get; set; }

        /// <summary>
        /// A list containing entries to the telemeters that want to publish data to the cloud.
        /// </summary>
        protected readonly List<ITelemeter> RegisteredTelemeters;

        /// <summary>
        /// The logging instance, if it's null then the module won't log anything.
        /// </summary>
        // ReSharper disable once UnassignedField.Global
        protected readonly ILogger Logger;

        /// <summary>
        /// Parameter less constructor, mainly used for testing.
        /// </summary>
        public TelemetryPublisher()
        {
        }

        /// <summary>
        /// Method that sends all data from the (<see cref="RegisteredTelemeters"/>) to the cloud.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task</returns>
        public abstract Task PublishAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Method that releases all managed resources.
        /// </summary>
        public abstract void Dispose();


        /// <summary>
        /// Method that adds a telemeter to the <see cref="RegisteredTelemeters"/> collection.
        /// The telemeter can register only once.
        /// </summary>
        /// <param name="t">The <see cref="ITelemeter"/></param>
        /// <returns>Returns true if the telemeter has registered successfully and false otherwise.</returns>
        public bool RegisterTelemeter(ITelemeter t)
        {
            if (RegisteredTelemeters.Contains(t)) return false;
            RegisteredTelemeters.Add(t);
            return true;
        }

        /// <summary>
        /// Method that deletes a telemeter from the <see cref="RegisteredTelemeters"/> collection.
        /// </summary>
        /// <param name="t">The <see cref="ITelemeter"/></param>
        /// <returns>Returns true if the telemeter has unregistered successfully and false otherwise.</returns>
        public bool UnRegisterTelemeter(ITelemeter t)
        {
            if (!RegisteredTelemeters.Contains(t)) return false;
            RegisteredTelemeters.Remove(t);
            return true;
        }

        /// <summary>
        /// Constructor for <see cref="TelemetryPublisher"/>.
        /// </summary>
        /// <param name="opts">TelemetryPublisher options, see: <see cref="TelemetryPublisherBuilderOptions"/></param>
        protected TelemetryPublisher(TelemetryPublisherBuilderOptions opts)
        {
            ConnectionString = opts.ConnectionString;
            TelemetrySource = opts.TelemetrySource;
            Logger = opts.Logger;
            RegisteredTelemeters = new List<ITelemeter>(5);
        }

        /// <summary>
        /// Iterates through the registered telemeters and returns the telemetry data as dictionary.
        /// It also adds metadata information such as: source and timestamp.
        /// </summary>
        /// <returns>A dictionary containing all telemetry data.</returns>
        protected Dictionary<string, object> GetTelemetry()
        {
            var data = new List<Dictionary<string, object>>();
            foreach (var telemeter in RegisteredTelemeters)
            {
                var telemetryData = telemeter.GetTelemetryData();
                if (telemetryData == null)
                {
                    Logger?.LogWarning($"Warning! Data for {telemeter.GetIdentifier()} is null!");
                    continue;
                }

                telemetryData["_id"] = telemeter.GetIdentifier();
                data.Add(telemetryData);
            }

            var metadata = new Dictionary<string, object>
            {
                ["source"] = TelemetrySource ?? nameof(TelemetryPublisher),
                ["timestamp"] = DateTime.Now,
                ["data"] = data.ToArray()
            };
            return metadata;
        }
    }
}