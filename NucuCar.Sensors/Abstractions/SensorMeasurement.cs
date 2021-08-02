using System.Diagnostics.CodeAnalysis;

namespace NucuCar.Sensors.Abstractions
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class SensorMeasurement
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Value { get; set; }

        public SensorMeasurement(string name, string unit, double value)
        {
            Name = name;
            Unit = unit;
            Value = value;
        }
    }
}