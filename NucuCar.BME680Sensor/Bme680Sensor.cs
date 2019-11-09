using System;
using System.Device.I2c;
using System.Threading.Tasks;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Units;
using Microsoft.Extensions.Logging;

namespace NucuCar.BME680Sensor
{
    public class Bme680Sensor : IDisposable
    {
        private readonly ILogger _logger;
        private I2cConnectionSettings _i2CSettings;
        private I2cDevice _i2CDevice;
        private Bme680 _bme680;
        private Bme680Measurement _measurement;
        private Bme680SensorState _sensorState;

        public Bme680Sensor(ILogger logger)
        {
            _logger = logger;
            InitializeSensor();
        }
        
        internal void InitializeSensor()
        {
            try
            {
                /* Connect to default i2c address 0x76 */
                _i2CSettings = new I2cConnectionSettings(1, Bme680.DefaultI2cAddress);
                _i2CDevice = I2cDevice.Create(_i2CSettings);
                _bme680 = new Bme680(_i2CDevice);
                
                /* Initialize measurement */
                _measurement = new Bme680Measurement();
                _sensorState = Bme680SensorState.Initialized;
                
                _logger.LogInformation($"{DateTimeOffset.Now}:BME680 Sensor initialization OK.");
            }
            catch (System.IO.IOException e)
            {
                _logger.LogError($"{DateTimeOffset.Now}:BME680 Sensor initialization FAIL.");
                _logger.LogTrace(e.Message);
                _sensorState = Bme680SensorState.Error;
            }
        }

        internal async Task TakeMeasurement()
        {
            if (_sensorState != Bme680SensorState.Initialized)
            {
                _logger.LogWarning($"{DateTimeOffset.Now}:BME680: Attempting to take measurement while sensor is not initialized!");
                _measurement.SetMeasurement(Temperature.FromCelsius(-1), -1, -1);
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

        // TODO: Make gRpc accessible.
        public Bme680Measurement GetMeasurement()
        {
            return _measurement;
        }

        // TODO: Make gRpc accessible.
        public Bme680SensorState GetState()
        {
            return _sensorState;
        }

        public void Dispose()
        {
            _bme680?.Dispose();
        }
    }


}