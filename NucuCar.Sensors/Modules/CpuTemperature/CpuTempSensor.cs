using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NucuCar.Sensors.Abstractions;

namespace NucuCar.Sensors.Modules.CpuTemperature
{
    public class CpuTempSensor : GenericTelemeterSensor, ISensor<CpuTempSensor>
    {
        private readonly Iot.Device.CpuTemperature.CpuTemperature _cpuTemperature;
        private double _lastTemperatureCelsius;

        public CpuTempSensor()
        {
        }
        
        public CpuTempSensor(ILogger<CpuTempSensor> logger, IOptions<CpuTempConfig> options)
        {
            Logger = logger;
            if (options.Value.Enabled)
            {
                CurrentState = SensorStateEnum.Uninitialized;
                _cpuTemperature = new Iot.Device.CpuTemperature.CpuTemperature();
                Object = this;
                TelemetryEnabled = options.Value.Telemetry;
            }
            else
            {
                Logger?.LogDebug("CpuTempSensor is disabled!");
                CurrentState = SensorStateEnum.Disabled;
            }
        }

        public override void Initialize()
        {    
            if (CurrentState == SensorStateEnum.Initialized || CurrentState == SensorStateEnum.Disabled)
            {
                return;
            }
            CurrentState = _cpuTemperature.IsAvailable ? SensorStateEnum.Initialized : SensorStateEnum.Error;
        }

        public override Task TakeMeasurementAsync()
        {
            if (CurrentState == SensorStateEnum.Initialized)
            {
                _lastTemperatureCelsius = _cpuTemperature.Temperature.DegreesCelsius;
                if (double.IsNaN(_lastTemperatureCelsius))
                {
                    CurrentState = SensorStateEnum.Error;
                    _lastTemperatureCelsius = double.NaN;
                }
                else
                {
                    _lastTemperatureCelsius = Math.Round(_lastTemperatureCelsius, 2);
                }
            }
            Logger?.LogInformation("CPU Temperature {CpuTemperature} \u00B0C", _lastTemperatureCelsius);
            return Task.FromResult(_lastTemperatureCelsius);
        }

        public override SensorResponse GetMeasurement()
        {
            return new SensorResponse()
            {
                SensorId = GetIdentifier(),
                State = CurrentState,
                Data = new List<SensorMeasurement>
                {
                    new SensorMeasurement("temperature", "celsius", _lastTemperatureCelsius)
                }
            };
        }

        public override SensorStateEnum GetState()
        {
            return CurrentState;
        }

        public override string GetIdentifier()
        {
            return "CpuTemperature";
        }

        public override JObject GetTelemetryJson()
        {
            JObject returnValue = null;
            if (!double.IsNaN(_lastTemperatureCelsius) && TelemetryEnabled)
            {
                returnValue = new JObject
                {
                    ["sensor_state"] = GetState().ToString(),
                    ["cpu_temperature"] = _lastTemperatureCelsius,
                };
            }

            return returnValue;
        }

        public override bool IsTelemetryEnabled()
        {
            return TelemetryEnabled;
        }

        public CpuTempSensor Object { get; }
    }
}