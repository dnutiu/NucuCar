using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NucuCar.Domain.Telemetry
{
    public abstract class TelemetryPublisher : ITelemetryPublisher
    {
        protected readonly List<ITelemeter> RegisteredTelemeters;
        // ReSharper disable once UnassignedField.Global
        public ILogger Logger;

        protected TelemetryPublisher()
        {
            RegisteredTelemeters = new List<ITelemeter>(5);
        }
        
        public abstract void Start();

        public abstract Task StartAsync();
        public abstract bool Publish(int timeout);
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
    }
}