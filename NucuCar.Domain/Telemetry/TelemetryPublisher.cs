using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NucuCar.Domain.Telemetry
{
    public abstract class TelemetryPublisher : ITelemetryPublisher, IDisposable
    {
        protected string ConnectionString { get; set; }
        protected string TelemetrySource { get; set; }
        protected readonly List<ITelemeter> RegisteredTelemeters;
        // ReSharper disable once UnassignedField.Global
        protected readonly ILogger Logger;

        protected TelemetryPublisher(TelemetryPublisherBuilderOptions opts)
        {
            ConnectionString = opts.ConnectionString;
            TelemetrySource = opts.TelemetrySource;
            Logger = opts.Logger;
            RegisteredTelemeters = new List<ITelemeter>(5);
        }

        public abstract Task PublishAsync(CancellationToken cancellationToken);
        public bool RegisterTelemeter(ITelemeter t)
        {
            if (RegisteredTelemeters.Contains(t)) return false;
            RegisteredTelemeters.Add(t);
            return true;

        }

        public bool UnRegisterTelemeter(ITelemeter t)
        {
            if (!RegisteredTelemeters.Contains(t)) return false;
            RegisteredTelemeters.Remove(t);
            return true;
        }

        public abstract void Dispose();
    }
}