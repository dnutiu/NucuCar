using NucuCar.Domain.Sensors;

namespace NucuCar.Sensors
{
    public interface ISensor<out TSensor> where TSensor : GenericTelemeterSensor
    {
        TSensor Object { get; }
    }
}