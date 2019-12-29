using System.Collections.Generic;
using System.Threading.Tasks;
using Iot.Device.CpuTemperature;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NucuCar.Domain.Sensors;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.Health
{
    public class CpuTempSensor : GenericTelemeterSensor, ISensor<CpuTempSensor>
    {
        private readonly CpuTemperature _cpuTemperature;
        private double _lastTemperatureCelsius;

        public CpuTempSensor()
        {
        }
        
        public CpuTempSensor(IOptions<CpuTempConfig> options)
        {
            if (options.Value.Enabled)
            {
                CurrentState = SensorStateEnum.Uninitialized;
                _cpuTemperature = new CpuTemperature();
                Object = this;
                TelemetryEnabled = options.Value.Telemetry;
            }
            else
            {
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
                _lastTemperatureCelsius = _cpuTemperature.Temperature.Celsius;
                if (double.IsNaN(_lastTemperatureCelsius))
                {
                    CurrentState = SensorStateEnum.Error;
                    _lastTemperatureCelsius = double.NaN;
                }
            }

            return Task.FromResult(_lastTemperatureCelsius);
        }

        public override NucuCarSensorResponse GetMeasurement()
        {
            var jsonResponse = JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                ["cpu_temperature"] = _lastTemperatureCelsius,
            });
            return new NucuCarSensorResponse()
            {
                State = CurrentState,
                JsonData = jsonResponse
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

        public override Dictionary<string, object> GetTelemetryData()
        {
            Dictionary<string, object> returnValue = null;
            if (!double.IsNaN(_lastTemperatureCelsius) && TelemetryEnabled)
            {
                returnValue = new Dictionary<string, object>
                {
                    ["sensor_state"] = CurrentState,
                    ["cpu_temperature"] = _lastTemperatureCelsius,
                };
            }

            return returnValue;
        }

        public CpuTempSensor Object { get; }
    }
}