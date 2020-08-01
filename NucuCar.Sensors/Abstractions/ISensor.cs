namespace NucuCar.Sensors.Abstractions
{
    public interface ISensor<out TSensor> where TSensor : GenericTelemeterSensor
    {
        TSensor Object { get; }
    }
}