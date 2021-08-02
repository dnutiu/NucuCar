using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;

namespace NucuCar.Sensors.Modules.Heartbeat
{
    public class HeartbeatSensor : GenericTelemeterSensor, ISensor<HeartbeatSensor>
    {
        public HeartbeatSensor()
        {
        }
        
        public HeartbeatSensor(ILogger<HeartbeatSensor> logger, IOptions<HeartbeatConfig> options)
        {
            Logger = logger;
            if (options.Value.Enabled)
            {
                CurrentState = SensorStateEnum.Uninitialized;
                Object = this;
                TelemetryEnabled = options.Value.Telemetry;
            }
            else
            {
                Logger?.LogDebug("HeartbeatSensor is disabled!");
                CurrentState = SensorStateEnum.Disabled;
            }
        }
        
        public override void Initialize()
        {
            Logger?.LogDebug("Heartbeat sensor initialized!");
            CurrentState = SensorStateEnum.Initialized;
        }

        public override Task TakeMeasurementAsync()
        {
            return Task.CompletedTask;
        }

        public override NucuCarSensorResponse GetMeasurement()
        {
            return new NucuCarSensorResponse()
            {
                SensorId = GetIdentifier(),
                State = CurrentState,
                Data = new List<SensorMeasurement>()
            };
        }

        public override SensorStateEnum GetState()
        {
            return CurrentState;
        }

        public override string GetIdentifier()
        {
            return "Heartbeat";
        }

        public override Dictionary<string, object> GetTelemetryData()
        {
            var returnValue = new Dictionary<string, object>
                {
                    ["sensor_state"] = CurrentState,
                    ["value"] = 1,
                };

            return returnValue;
        }

        public override bool IsTelemetryEnabled()
        {
            return TelemetryEnabled;
        }

        public HeartbeatSensor Object { get; }
    }
}