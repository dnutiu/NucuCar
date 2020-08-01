namespace NucuCar.Sensors.Modules.Environment.Bmxx80.Units
{
    public struct Temperature
    {
        private const double KelvinOffset = 273.15;
        private const double FahrenheitOffset = 32.0;
        private const double FahrenheitRatio = 1.8;
        private double _celsius;

        private Temperature(double celsius)
        {
            _celsius = celsius;
        }

        /// <summary>
        /// Temperature in Celsius
        /// </summary>
        public double Celsius => _celsius;

        /// <summary>
        /// Temperature in Fahrenheit
        /// </summary>
        public double Fahrenheit => FahrenheitRatio * _celsius + FahrenheitOffset;

        /// <summary>
        /// Temperature in Kelvin
        /// </summary>
        public double Kelvin => _celsius + KelvinOffset;

        /// <summary>
        /// Creates Temperature instance from temperature in Celsius
        /// </summary>
        /// <param name="value">Temperature value in Celsius</param>
        /// <returns>Temperature instance</returns>
        public static Temperature FromCelsius(double value)
        {
            return new Temperature(value);
        }

        /// <summary>
        /// Creates Temperature instance from temperature in Fahrenheit
        /// </summary>
        /// <param name="value">Temperature value in Fahrenheit</param>
        /// <returns>Temperature instance</returns>
        public static Temperature FromFahrenheit(double value)
        {
            return new Temperature((value - FahrenheitOffset) / FahrenheitRatio);
        }

        /// <summary>
        /// Creates Temperature instance from temperature in Kelvin
        /// </summary>
        /// <param name="value">Temperature value in Kelvin</param>
        /// <returns>Temperature instance</returns>
        public static Temperature FromKelvin(double value)
        {
            return new Temperature(value - KelvinOffset);
        }
    }
}