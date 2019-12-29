using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Threading.Tasks;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NucuCar.Domain.Sensors;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.Environment
{
    internal class Bme680MeasurementData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double VolatileOrganicCompounds { get; set; }
    }
    
    /// <summary>
    /// Abstraction for the BME680 sensor.
    /// See: https://www.bosch-sensortec.com/bst/products/all_products/bme680
    /// </summary>
    public class Bme680Sensor : GenericTelemeterSensor, IDisposable, ISensor<Bme680Sensor>
    {
        private I2cConnectionSettings _i2CSettings;
        private I2cDevice _i2CDevice;
        private Bme680 _bme680;
        private Bme680MeasurementData _lastMeasurement;

        public Bme680Sensor()
        {
        }

        public Bme680Sensor(ILogger<Bme680Sensor> logger, IOptions<Bme680Config> options)
        {
            SensorStateEnum = SensorStateEnum.Uninitialized;
            Logger = logger;
            if (!options.Value.Enabled)
            {
                Logger?.LogInformation("BME680 Sensor is disabled!");
                SensorStateEnum = SensorStateEnum.Disabled;
            }

            TelemetryEnabled = options.Value.Telemetry;

            Object = this;
        }

        public override NucuCarSensorResponse GetMeasurement()
        {
            var jsonResponse = JsonConvert.SerializeObject(_lastMeasurement);
            return new NucuCarSensorResponse
            {
                State = GetState(),
                JsonData = jsonResponse
            };
        }

        public override SensorStateEnum GetState()
        {
            return SensorStateEnum;
        }

        public void Dispose()
        {
            _bme680?.Dispose();
        }

        public override void Initialize()
        {
            if (SensorStateEnum == SensorStateEnum.Initialized || SensorStateEnum == SensorStateEnum.Disabled)
            {
                return;
            }

            _lastMeasurement = new Bme680MeasurementData();

            try
            {
                /* Connect to default i2c address 0x76 */
                _i2CSettings = new I2cConnectionSettings(1, Bme680.DefaultI2cAddress);
                _i2CDevice = I2cDevice.Create(_i2CSettings);
                _bme680 = new Bme680(_i2CDevice);

                /* Initialize measurement */
                _bme680.Reset();
                _bme680.SetHumiditySampling(Sampling.UltraLowPower);
                _bme680.SetTemperatureSampling(Sampling.UltraHighResolution);
                _bme680.SetPressureSampling(Sampling.UltraLowPower);
                SensorStateEnum = SensorStateEnum.Initialized;

                Logger?.LogInformation($"{DateTimeOffset.Now}:BME680 Sensor initialization OK.");
            }
            catch (System.IO.IOException e)
            {
                Logger?.LogError($"{DateTimeOffset.Now}:BME680 Sensor initialization FAIL.");
                Logger?.LogTrace(e.Message);
                SensorStateEnum = SensorStateEnum.Error;
            }
        }

        public override async Task TakeMeasurementAsync()
        {
            if (SensorStateEnum != SensorStateEnum.Initialized)
            {
                throw new InvalidOperationException("Can't take measurement on uninitialized sensor!");
            }

            /* Force the sensor to take a measurement. */
            _bme680.SetPowerMode(Bme680PowerMode.Forced);

            _lastMeasurement.Temperature = (await _bme680.ReadTemperatureAsync()).Celsius;
            _lastMeasurement.Pressure = await _bme680.ReadPressureAsync();
            _lastMeasurement.Humidity = await _bme680.ReadHumidityAsync();
            _lastMeasurement.VolatileOrganicCompounds = 0.0; // Not implemented.

            Logger?.LogDebug($"{DateTimeOffset.Now}:BME680: reading");
            Logger?.LogInformation(
                $"temperature:{_lastMeasurement.Temperature:N2} \u00B0C|" +
                $"pressure:{_lastMeasurement.Pressure:N2} hPa|" +
                $"humidity:{_lastMeasurement.Humidity:N2} %rH|" +
                $"voc:{_lastMeasurement.VolatileOrganicCompounds}");
        }

        public override string GetIdentifier()
        {
            return "Bme680-Sensor";
        }

        public override Dictionary<string, object> GetTelemetryData()
        {
            Dictionary<string, object> returnValue = null;
            if (_lastMeasurement != null && TelemetryEnabled)
            {
                returnValue = new Dictionary<string, object>
                {
                    ["sensor_state"] = SensorStateEnum,
                    ["temperature"] = _lastMeasurement.Temperature,
                    ["humidity"] = _lastMeasurement.Humidity,
                    ["pressure"] = _lastMeasurement.Pressure,
                    ["voc"] = _lastMeasurement.VolatileOrganicCompounds
                };
            }

            return returnValue;
        }

        public Bme680Sensor Object { get; }
    }
}