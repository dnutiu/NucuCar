using NucuCar.Domain.Telemetry;

namespace NucuCar.Sensors
{
    public interface ISensor<out TSensor> where TSensor : class, new()
    {
        TSensor Object { get; }
    }
}