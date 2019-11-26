using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Threading.Tasks;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Domain.Telemetry;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.EnvironmentSensor
{
    /// <summary>
    /// Abstraction for the BME680 sensor.
    /// See: https://www.bosch-sensortec.com/bst/products/all_products/bme680
    /// </summary>
    public class Bme680Sensor : IDisposable, ITelemeter
    {
        private readonly ILogger _logger;
        private I2cConnectionSettings _i2CSettings;
        private I2cDevice _i2CDevice;
        private Bme680 _bme680;
        private EnvironmentSensorMeasurement _lastMeasurement;
        private SensorStateEnum _sensorStateEnum;

        public Bme680Sensor(ILogger<Bme680Sensor> logger, IOptions<Bme680Config> options)
        {
            _sensorStateEnum = SensorStateEnum.Uninitialized;
            _logger = logger;
            if (options.Value.ServiceEnabled)
            {
                InitializeSensor();
            }
            else
            {
                _logger?.LogInformation("BME680 Sensor is disabled!");
                _sensorStateEnum = SensorStateEnum.Disabled;
            }
        }

        public EnvironmentSensorMeasurement GetMeasurement()
        {
            return _lastMeasurement;
        }

        public SensorStateEnum GetState()
        {
            return _sensorStateEnum;
        }

        public void Dispose()
        {
            _bme680?.Dispose();
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
                _lastMeasurement = new EnvironmentSensorMeasurement();
                _bme680.Reset();
                _bme680.SetHumiditySampling(Sampling.UltraLowPower);
                _bme680.SetTemperatureSampling(Sampling.UltraHighResolution);
                _bme680.SetPressureSampling(Sampling.UltraLowPower);
                _sensorStateEnum = SensorStateEnum.Initialized;

                _logger?.LogInformation($"{DateTimeOffset.Now}:BME680 Sensor initialization OK.");
            }
            catch (System.IO.IOException e)
            {
                _logger?.LogError($"{DateTimeOffset.Now}:BME680 Sensor initialization FAIL.");
                _logger?.LogTrace(e.Message);
                _sensorStateEnum = SensorStateEnum.Error;
            }
        }

        internal async Task TakeMeasurement()
        {
            if (_sensorStateEnum != SensorStateEnum.Initialized)
            {
                _logger?.LogWarning(
                    $"{DateTimeOffset.Now}:BME680: Attempting to take measurement while sensor is not initialized!");
                return;
            }

            /* Force the sensor to take a measurement. */
            _bme680.SetPowerMode(Bme680PowerMode.Forced);

            _lastMeasurement.Temperature = (await _bme680.ReadTemperatureAsync()).Celsius;
            _lastMeasurement.Pressure = await _bme680.ReadPressureAsync();
            _lastMeasurement.Humidity = await _bme680.ReadHumidityAsync();

            _logger?.LogInformation($"{DateTimeOffset.Now}:BME680: reading");
            _logger?.LogInformation(
                $"{_lastMeasurement.Temperature:N2} \u00B0C | {_lastMeasurement.Pressure:N2} hPa | {_lastMeasurement.Humidity:N2} %rH");
        }

        public string GetIdentifier()
        {
            return nameof(EnvironmentSensor);
        }

        public Dictionary<string, object> GetTelemetryData()
        {
            Dictionary<string, object> returnValue = null;
            if (_lastMeasurement != null)
            {
                returnValue = new Dictionary<string, object>
                {
                    ["temperature"] = _lastMeasurement.Temperature,
                    ["humidity"] = _lastMeasurement.Humidity,
                    ["pressure"] = _lastMeasurement.Pressure,
                    ["voc"] = _lastMeasurement.VolatileOrganicCompound
                };
            }

            return returnValue;
        }
    }
}