using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NucuCar.Sensors.Telemetry
{
    public class TelemetryService : IDisposable
    {
        private readonly List<ITelemetrySensor> _registeredSensors;

        /* Singleton Instance */
        public static TelemetryService Instance { get; } = new TelemetryService();

        static TelemetryService()
        {
        }

        private TelemetryService()
        {
            _registeredSensors = new List<ITelemetrySensor>(5);
        }

        public void Dispose()
        {
        }

        public async Task PublishData()
        {
            foreach (var sensor in _registeredSensors)
            {
                var data = sensor.GetTelemetryData();
                await UploadData(data);
            }
        }

        private async Task UploadData(Dictionary<string, double> data)
        {
            foreach (var entry in data)
            {
                // TODO: Publish data to IoTCore
            }
        }

        public void RegisterSensor(ITelemetrySensor sensor)
        {
            _registeredSensors.Add(sensor);
        }

        public void UnregisterSensor(ITelemetrySensor sensor)
        {
            _registeredSensors.Remove(sensor);
        }
    }
}