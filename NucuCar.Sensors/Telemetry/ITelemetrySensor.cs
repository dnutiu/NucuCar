using System.Collections.Generic;

namespace NucuCar.Sensors.Telemetry
{
    public interface ITelemetrySensor
    {
        /* Dictionary containing the topic and the value */
        Dictionary<string, double> GetTelemetryData();
    }
}