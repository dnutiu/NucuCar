using Newtonsoft.Json.Linq;
using NucuCar.Telemetry.Publishers;

namespace NucuCar.Telemetry.Abstractions
{
    /// <summary>
    /// Interface that specifies that the component implementing it is willing to provide telemetry data and can be
    /// registered to a publisher such as <see cref="BasePublisher"/>.
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
        /// When implementing this function you should return null if the telemetry is disabled.
        /// See: <see cref="IsTelemetryEnabled"/>
        /// </summary>
        /// <returns>The telemetry data as a Newtonsoft JObject.</returns>
        JObject GetTelemetryJson();

        /// <summary>
        /// This function should return whether the sensor has telemetry enabled or not.
        /// A value of true indicates that the sensor has enabled telemetry, and a value of false indicates
        /// that telemetry is not enabled.
        /// </summary>
        /// <returns>A boolean indicating if the sensor has enabled telemetry.</returns>
        bool IsTelemetryEnabled();
    }
}