using System.Collections.Generic;

namespace NucuCar.Domain.Telemetry
{
    /// <summary>
    /// Interface that specifies that the component implementing it is willing to provide telemetry data and can be
    /// registered to a publisher such as <see cref="TelemetryPublisherAzure"/>.
    /// </summary>
    public interface ITelemeter
    {
        /// <summary>
        /// This function should return an identifier that identifies the component providing the telemetry data.
        /// </summary>
        /// <returns>An identifier for the telemetry source.</returns>
        string GetIdentifier();

        /// <summary>
        /// This function should return a dictionary containing the telemetry data.
        /// </summary>
        /// <returns>The telemetry data. It should be JSON serializable.</returns>
        Dictionary<string, object> GetTelemetryData();
    }
}