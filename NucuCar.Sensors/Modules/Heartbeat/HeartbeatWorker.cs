﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;

namespace NucuCar.Sensors.Modules.Heartbeat
{
    public class HeartbeatWorker : SensorWorker
    {
        public HeartbeatWorker(ILogger<HeartbeatWorker> logger, Telemetry.Telemetry telemetry,
            ISensor<HeartbeatSensor> sensor, IOptions<HeartbeatConfig> options)
        {
            Logger = logger;
            MeasurementInterval = options.Value.MeasurementInterval;
            TelemetryPublisher = telemetry.Publisher;
            Sensor = sensor.Object;
        }
    }
}