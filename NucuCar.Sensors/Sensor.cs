using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors
{
    public class Sensor<T> : ISensor<T> where T : class, ITelemeter
    {
        public T Object { get; }
    }
}