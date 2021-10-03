using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace NucuCar.Telemetry
{
    /**
     * <see cref="DataAggregate"/> is an entity holding the telemetry data body.
     * It contains the telemetry data from all the telemeters.
     */
    public class DataAggregate
    {
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
        public List<JObject> Data { get; set; }
        
        public DataAggregate(string source, List<JObject> data)
        {
            Source = source;
            Data = data;
            Timestamp = DateTime.UtcNow;
        }
    }
}