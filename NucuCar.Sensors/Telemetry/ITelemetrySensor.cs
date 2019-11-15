using System;
using System.Collections.Generic;

namespace NucuCar.Sensors.Telemetry
{
    public interface ITelemetrySensor
    {
        string GetIdentifier();
        /* Dictionary containing the topic and the value */
        Dictionary<string, double> GetTelemetryData();
    }
}