using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NucuCar.Domain.Telemetry
{
    public interface ITelemetryPublisher
    {
        void Start();
        Task StartAsync();
        bool RegisterTelemeter(ITelemeter t);
        bool UnRegisterTelemeter(ITelemeter t);
        Task PublishAsync(CancellationToken cancellationToken);
        bool Publish(int timeout);
    }
}