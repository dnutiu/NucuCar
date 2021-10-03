using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Telemetry.Abstractions;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace NucuCar.Telemetry
{
    public class PublisherProxy : ITelemetryPublisher
    {
        // TODO: Add support for chaining publishers.
        private ITelemetryPublisher Publisher { get; }

        /// <summary>
        /// Class used together with the DI, holds a Publisher instance that's being create by options from
        /// TelemetryConfig.
        /// </summary>
        public PublisherProxy()
        {
        }

        public PublisherProxy(ILogger<PublisherProxy> logger, IOptions<Config> options)
        {
            if (options.Value.ServiceEnabled)
            {
                Publisher = PublisherFactory.Create(options.Value.Publisher, options.Value.ConnectionString,
                    "NucuCar.Sensors", logger);
            }
            else
            {
                Publisher = null;
            }
        }

        public Task PublishAsync(CancellationToken cancellationToken)
        {
            return Publisher.PublishAsync(cancellationToken);
        }

        public bool RegisterTelemeter(ITelemeter t)
        {
            return Publisher.RegisterTelemeter(t);
        }

        public bool UnRegisterTelemeter(ITelemeter t)
        {
            return Publisher.UnRegisterTelemeter(t);
        }
    }
}