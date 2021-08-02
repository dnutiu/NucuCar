using System.Collections.Generic;

namespace NucuCar.Sensors.Abstractions
{
    public class NucuCarSensorResponse
    {
        // TODO: Fix names in NucuCar.Sensors.Modules.
        public SensorStateEnum State;
        public string SensorId;
        public List<SensorMeasurement> Data;
    }
}