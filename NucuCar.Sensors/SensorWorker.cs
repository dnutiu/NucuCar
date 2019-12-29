using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Sensors;
using NucuCar.Domain.Telemetry;
using NucuCarSensorsProto;

namespace NucuCar.Sensors
{
    /// <summary>
    /// Generic Sensor background service worker. Each sensor's worker class should extend this one.
    /// It does periodic reads from the sensors and publishes telemetry data if the option is enabled.
    /// </summary>
    public abstract class SensorWorker : BackgroundService
    {
        private int _intializationDelay = 10000;
        protected int MeasurementInterval;
        protected ILogger Logger;
        protected TelemetryPublisher TelemetryPublisher;
        protected GenericTelemeterSensor Sensor;


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sensorIdentifier = Sensor.GetIdentifier();
            Logger.LogInformation($"Starting sensor worker for {sensorIdentifier}");
            TelemetryPublisher?.RegisterTelemeter(Sensor);

            Sensor.Initialize();
            while (!stoppingToken.IsCancellationRequested)
            {
                var sensorState = Sensor.GetState();
                /* If sensor is ok attempt to read. */
                if (sensorState == SensorStateEnum.Initialized)
                {
                    await Sensor.TakeMeasurementAsync();
                }
                /* Else attempt to re-initialize. */
                else if (sensorState == SensorStateEnum.Uninitialized ||
                         sensorState == SensorStateEnum.Error)
                {
                    Logger.LogWarning(
                        $"{sensorIdentifier} is in {sensorState}! Attempting to re-initialize in {_intializationDelay}ms.");
                    _intializationDelay += 10000;
                    await Task.Delay(_intializationDelay, stoppingToken);
                    Sensor.Initialize();
                }
                else if (sensorState == SensorStateEnum.Disabled)
                {
                    // Break from while.
                    break;
                }

                await Task.Delay(MeasurementInterval, stoppingToken);
            }

            TelemetryPublisher?.UnRegisterTelemeter(Sensor);
        }
    }
}