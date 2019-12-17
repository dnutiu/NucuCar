using System.Collections.Generic;
using NucuCar.Domain.Telemetry;

namespace NucuCar.Domain.Sensors
{
    /// <summary>
    /// The GenericSensor is an abstract class, which provides a base for abstracting hardware sensors
    /// with telemetry support.
    /// See: <see cref="ITelemeter"/>
    /// See: <see cref="GenericSensor"/>
    /// </summary>
    public abstract class GenericTelemeterSensor : GenericSensor, ITelemeter
    {
        public abstract string GetIdentifier();
        public abstract Dictionary<string, object> GetTelemetryData();
    }
}