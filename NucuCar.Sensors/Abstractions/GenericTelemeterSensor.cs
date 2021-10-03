using System.Collections.Generic;
using NucuCar.Telemetry.Abstractions;

namespace NucuCar.Sensors.Abstractions
{
    /// <summary>
    /// The GenericSensor is an abstract class, which provides a base for abstracting hardware sensors
    /// with telemetry support.
    /// See: <see cref="ITelemeter"/>
    /// See: <see cref="NucuCar.Sensors.Abstractions.GenericSensor"/>
    /// </summary>
    public abstract class GenericTelemeterSensor : GenericSensor, ITelemeter
    {
        protected bool TelemetryEnabled;
        public abstract string GetIdentifier();
        public abstract Dictionary<string, object> GetTelemetryJson();
        public abstract bool IsTelemetryEnabled();
    }
}