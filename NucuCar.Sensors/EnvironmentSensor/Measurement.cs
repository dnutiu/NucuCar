using Iot.Units;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public struct Measurement
    {
        public Temperature Temperature { get; set; }
        public double Pressure { get; set; }
        public double Humidity { get; set; }

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