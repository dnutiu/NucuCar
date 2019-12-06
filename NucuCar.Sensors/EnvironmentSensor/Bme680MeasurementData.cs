namespace NucuCar.Sensors.EnvironmentSensor
{
    public class Bme680MeasurementData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double VolatileOrganicCompounds { get; set; }
    }
}