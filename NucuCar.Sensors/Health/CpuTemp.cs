using System.Collections.Generic;
using System.Threading.Tasks;
using Iot.Device.CpuTemperature;
using Newtonsoft.Json;
using NucuCar.Domain.Sensors;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.Health
{
    public class CpuTemp : GenericTelemeterSensor, ISensor<CpuTemp>
    {
        private readonly CpuTemperature _cpuTemperature;
        private double _lastTemperatureCelsius;

        public CpuTemp()
        {
            _cpuTemperature = new CpuTemperature();
            Object = this;
        }

        public override void Initialize()
        {
            CurrentState = SensorStateEnum.Initialized;
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

        public CpuTemp Object { get; }
    }
}