namespace NucuCar.Sensors.Abstractions
{
    public enum SensorStateEnum : ushort
    {
        Error = 0,
        Uninitialized = 1,
        Initialized = 2,
        Disabled = 3,
    }
}