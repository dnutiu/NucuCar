using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NucuCar.Sensors.Abstractions;
using Iot.Device.Bmxx80;
using UnitsNet;
using Bme680 = Iot.Device.Bmxx80.Bme680;
using Bme680PowerMode = Iot.Device.Bmxx80.PowerMode.Bme680PowerMode;
using Sampling = Iot.Device.Bmxx80.Sampling;

namespace NucuCar.Sensors.Modules.Environment
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
            CurrentState = SensorStateEnum.Uninitialized;
            Logger = logger;
            if (!options.Value.Enabled)
            {
                Logger?.LogDebug("BME680 Sensor is disabled!");
                CurrentState = SensorStateEnum.Disabled;
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
            return CurrentState;
        }

        public void Dispose()
        {
            _bme680?.Dispose();
        }

        public override void Initialize()
        {
            if (CurrentState == SensorStateEnum.Initialized || CurrentState == SensorStateEnum.Disabled)
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
                _bme680.TemperatureSampling = Sampling.HighResolution;
                _bme680.HumiditySampling = Sampling.HighResolution;
                _bme680.PressureSampling = Sampling.HighResolution;
                _bme680.HeaterProfile = Bme680HeaterProfile.Profile2;

                CurrentState = SensorStateEnum.Initialized;

                Logger?.LogInformation($"{DateTimeOffset.Now}:BME680 Sensor initialization OK.");
            }
            catch (System.IO.IOException e)
            {
                HandleInitializationException(e);
            }
            catch (ArgumentException e)
            {
                HandleInitializationException(e);
            }
        }

        public override async Task TakeMeasurementAsync()
        {
            if (CurrentState != SensorStateEnum.Initialized)
            {
                throw new InvalidOperationException("Can't take measurement on uninitialized sensor!");
            }

            _bme680.ConfigureHeatingProfile(Bme680HeaterProfile.Profile2,
                Temperature.FromDegreesCelsius(280), Duration.FromMilliseconds(80),
                Temperature.FromDegreesCelsius(_lastMeasurement.Temperature));
            var measurementDuration = _bme680.GetMeasurementDuration(_bme680.HeaterProfile);

            /* Force the sensor to take a measurement. */
            _bme680.SetPowerMode(Bme680PowerMode.Forced);
            await Task.Delay(measurementDuration.ToTimeSpan());

            _bme680.TryReadTemperature(out var temp);
            _bme680.TryReadHumidity(out var humidity);
            _bme680.TryReadPressure(out var pressure);
            _bme680.TryReadGasResistance(out var gasResistance);

            _lastMeasurement.Temperature = Math.Round(temp.DegreesCelsius, 2);
            _lastMeasurement.Pressure = Math.Round(pressure.Hectopascals, 2);
            _lastMeasurement.Humidity = Math.Round(humidity.Percent, 2);
            _lastMeasurement.VolatileOrganicCompounds = Math.Round(gasResistance.Kiloohms, 2);

            Logger?.LogDebug($"{DateTimeOffset.Now}:BME680: reading");
            Logger?.LogInformation(
                $"temperature:{_lastMeasurement.Temperature:N2} \u00B0C|" +
                $"pressure:{_lastMeasurement.Pressure:N2} hPa|" +
                $"humidity:{_lastMeasurement.Humidity:N2} %rH|" +
                $"voc:{_lastMeasurement.VolatileOrganicCompounds}");
        }

        public override string GetIdentifier()
        {
            return "Environment";
        }

        public override Dictionary<string, object> GetTelemetryData()
        {
            Dictionary<string, object> returnValue = null;
            if (_lastMeasurement != null && TelemetryEnabled)
            {
                returnValue = new Dictionary<string, object>
                {
                    ["sensor_state"] = CurrentState,
                    ["temperature"] = _lastMeasurement.Temperature,
                    ["humidity"] = _lastMeasurement.Humidity,
                    ["pressure"] = _lastMeasurement.Pressure,
                    ["voc"] = _lastMeasurement.VolatileOrganicCompounds
                };
            }

            return returnValue;
        }

        public override bool IsTelemetryEnabled()
        {
            return TelemetryEnabled;
        }

        public Bme680Sensor Object { get; }

        private void HandleInitializationException(Exception e)
        {
            Logger?.LogError($"{DateTimeOffset.Now}:BME680 Sensor initialization FAIL.");
            Logger?.LogDebug(e.Message);
            CurrentState = SensorStateEnum.Error;
        }
    }
}