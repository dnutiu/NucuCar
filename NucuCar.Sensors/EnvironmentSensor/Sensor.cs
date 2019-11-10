using System;
using System.Device.I2c;
using System.Threading.Tasks;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Microsoft.Extensions.Logging;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public class Sensor : IDisposable
    {
        private ILogger _logger;
        private I2cConnectionSettings _i2CSettings;
        private I2cDevice _i2CDevice;
        private Bme680 _bme680;
        private Measurement _measurement;
        private SensorStateEnum _sensorStateEnum;

        /* Singleton Instance */
        public static Sensor Instance { get; } = new Sensor();

        static Sensor()
        {
        }

        private Sensor()
        {
            _sensorStateEnum = SensorStateEnum.Uninitialized;
        }

        public Measurement GetMeasurement()
        {
            return _measurement;
        }

        public SensorStateEnum GetState()
        {
            return _sensorStateEnum;
        }

        public void Dispose()
        {
            _bme680?.Dispose();
        }

        internal void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        internal void InitializeSensor()
        {
            if (_sensorStateEnum == SensorStateEnum.Initialized)
            {
                return;
            }

            try
            {
                /* Connect to default i2c address 0x76 */
                _i2CSettings = new I2cConnectionSettings(1, Bme680.DefaultI2cAddress);
                _i2CDevice = I2cDevice.Create(_i2CSettings);
                _bme680 = new Bme680(_i2CDevice);

                /* Initialize measurement */
                _measurement = new Measurement();
                _bme680.Reset();
                _bme680.SetHumiditySampling(Sampling.UltraLowPower);
                _bme680.SetTemperatureSampling(Sampling.UltraHighResolution);
                _bme680.SetPressureSampling(Sampling.UltraLowPower);
                _sensorStateEnum = SensorStateEnum.Initialized;

                _logger.LogInformation($"{DateTimeOffset.Now}:BME680 Sensor initialization OK.");
            }
            catch (System.IO.IOException e)
            {
                _logger.LogError($"{DateTimeOffset.Now}:BME680 Sensor initialization FAIL.");
                _logger.LogTrace(e.Message);
                _sensorStateEnum = SensorStateEnum.Error;
            }
        }

        internal async Task TakeMeasurement()
        {
            if (_sensorStateEnum != SensorStateEnum.Initialized)
            {
                _logger.LogWarning(
                    $"{DateTimeOffset.Now}:BME680: Attempting to take measurement while sensor is not initialized!");
                return;
            }

            /* Force the sensor to take a measurement. */
            _bme680.SetPowerMode(Bme680PowerMode.Forced);

            var temperature = await _bme680.ReadTemperatureAsync();
            var pressure = await _bme680.ReadPressureAsync();
            var humidity = await _bme680.ReadHumidityAsync();
            _measurement.SetMeasurement(temperature, pressure, humidity);

            _logger.LogInformation($"{DateTimeOffset.Now}:BME680: reading");
            _logger.LogInformation(
                $"{temperature.Celsius:N2} \u00B0C | {pressure} hPa | {humidity:N2} %rH");
        }
    }
}