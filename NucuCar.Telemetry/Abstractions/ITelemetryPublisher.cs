using System.Threading;
using System.Threading.Tasks;

namespace NucuCar.Telemetry.Abstractions
{
    public interface ITelemetryPublisher
    {
        /// <summary>
        /// Publishes telemetry data.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task</returns>
        public abstract Task PublishAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Adds a telemeter to the collection.
        /// The telemeter can register only once.
        /// </summary>
        /// <param name="t">The <see cref="ITelemeter"/></param>
        /// <returns>Returns true if the telemeter has registered successfully and false otherwise.</returns>
        public bool RegisterTelemeter(ITelemeter t);

        /// <summary>
        /// Method that deletes a telemeter from the collection.
        /// </summary>
        /// <param name="t">The <see cref="ITelemeter"/></param>
        /// <returns>Returns true if the telemeter has unregistered successfully and false otherwise.</returns>
        public bool UnRegisterTelemeter(ITelemeter t);
    }
}