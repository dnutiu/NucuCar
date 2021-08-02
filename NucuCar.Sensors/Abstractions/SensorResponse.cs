using System.Collections.Generic;

namespace NucuCar.Sensors.Abstractions
{
    public class SensorResponse
    {
        public SensorStateEnum State;
        public string SensorId;
        public List<SensorMeasurement> Data;
    }
}