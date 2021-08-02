using System.Collections.Generic;

namespace NucuCar.Telemetry.Abstractions
{
    /// <summary>
    /// Interface that specifies that the component implementing it is willing to provide telemetry data and can be
    /// registered to a publisher such as <see cref="TelemetryPublisher"/>.
    /// </summary>
    public interface ITelemeter
    {
        /// <summary>
        /// This function should return an identifier that identifies the component providing the telemetry data.
        /// </summary>
        /// <returns>An identifier for the telemetry source.</returns>
        string GetIdentifier();

        // TODO: Perhaps here it's better if we return a string.
        /// <summary>
        /// This function should return a dictionary containing the telemetry data.
        /// When implementing this function you should return null if the telemetry is disabled.
        /// See: <see cref="IsTelemetryEnabled"/>
        /// </summary>
        /// <returns>The telemetry data. It should be JSON serializable.</returns>
        Dictionary<string, object> GetTelemetryData();

        /// <summary>
        /// This function should return whether the sensor has telemetry enabled or not.
        /// A value of true indicates that the sensor has enabled telemetry, and a value of false indicates
        /// that telemetry is not enabled.
        /// </summary>
        /// <returns>A boolean indicating if the sensor has enabled telemetry.</returns>
        bool IsTelemetryEnabled();
    }
}