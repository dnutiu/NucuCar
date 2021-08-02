using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NucuCar.Sensors.Abstractions;
using NucuCar.Telemetry.Abstractions;

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
        protected ITelemetryPublisher TelemetryPublisher;
        protected GenericTelemeterSensor Sensor;


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (Sensor == null)
            {
                Logger?.LogDebug("{Message}","Sensor is null, abandoning execution.");
                return;
            }

            var sensorIdentifier = Sensor.GetIdentifier();
            Logger?.LogInformation("Starting sensor worker for {SensorId}", sensorIdentifier);
            try
            {
                TelemetryPublisher?.RegisterTelemeter(Sensor);

                Sensor.Initialize();
                while (!stoppingToken.IsCancellationRequested)
                {
                    var sensorState = Sensor.GetState();
                    /* If sensor is ok attempt to read. */
                    if (sensorState == SensorStateEnum.Initialized)
                    {
                        Logger?.LogTrace("{SensorId} is taking a measurement!", sensorIdentifier);
                        await Sensor.TakeMeasurementAsync();
                    }
                    /* Else attempt to re-initialize. */
                    else if (sensorState == SensorStateEnum.Uninitialized ||
                             sensorState == SensorStateEnum.Error)
                    {
                        Logger?.LogWarning(
                            "{SensorId} is in {SensorState}! Attempting to re-initialize in {InitDelay}ms",
                            sensorIdentifier, sensorState, _intializationDelay);
                        _intializationDelay += 10000;
                        await Task.Delay(_intializationDelay, stoppingToken);
                        Sensor.Initialize();
                    }
                    else if (sensorState == SensorStateEnum.Disabled)
                    {
                        // Break from while.
                        Logger?.LogInformation("{SensorIdentifier} has been disabled!", sensorIdentifier);
                        break;
                    }

                    await Task.Delay(MeasurementInterval, stoppingToken);
                }

                TelemetryPublisher?.UnRegisterTelemeter(Sensor);
            }
            catch (TaskCanceledException)
            {
                Logger?.LogInformation("The SensorWorker task was canceled");
            }
            catch (Exception e)
            {
                Logger?.LogError("Unhandled exception in SensorWorker {SensorId}. {Type}: {Message}",
                    sensorIdentifier, e.GetType(), e.Message);
                Logger?.LogDebug("{StackTrace}", e.StackTrace);
            }
        }
    }
}