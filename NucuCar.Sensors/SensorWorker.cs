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
        protected int MeasurementInterval;
        protected ILogger Logger;
        protected TelemetryPublisher TelemetryPublisher;
        protected GenericTelemeterSensor Sensor;


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Starting sensor worker");
            TelemetryPublisher?.RegisterTelemeter(Sensor);

            Sensor.Initialize();
            while (!stoppingToken.IsCancellationRequested)
            {
                /* If sensor is ok attempt to read. */
                if (Sensor.GetState() == SensorStateEnum.Initialized)
                {
                    await Sensor.TakeMeasurementAsync();
                }
                /* Else attempt to re-initialize. */
                else if (Sensor.GetState() == SensorStateEnum.Uninitialized ||
                         Sensor.GetState() == SensorStateEnum.Error)
                {
                    await Task.Delay(10000, stoppingToken);
                    Sensor.Initialize();
                }

                await Task.Delay(MeasurementInterval, stoppingToken);
            }

            TelemetryPublisher?.UnRegisterTelemeter(Sensor);
        }
    }
}