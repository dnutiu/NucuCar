using Iot.Units;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public struct Measurement
    {
        public Temperature Temperature { get; private set; }
        public double Pressure { get; private set; }
        public double Humidity { get; private set; }

        public Measurement(Temperature temperature, double pressure, double humidity) : this()
        {
            SetMeasurement(temperature, pressure, humidity);
        }

        public void SetMeasurement(Temperature temperature, double pressure, double humidity)
        {
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
        }
    }
}