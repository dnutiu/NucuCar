using System.Collections.Generic;

namespace NucuCar.Domain.Telemetry
{
    public interface ITelemeter
    {
        string GetIdentifier();
        /* Dictionary containing the topic and the value */
        Dictionary<string, object> GetTelemetryData();
    }
}