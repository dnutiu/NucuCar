using Iot.Units;

namespace NucuCar.BME680Sensor
{
    public struct Bme680Measurement
    {
        public Temperature Temperature { get; set; }
        public double Pressure { get; set; }
        public double Humidity { get; set; }

        public Bme680Measurement(Temperature temperature, double pressure, double humidity) : this()
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