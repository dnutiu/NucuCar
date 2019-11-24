using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.EnvironmentSensor
{
    /// <summary>
    /// EnvironmentSensor's gRPC service.
    /// It allows reading the sensor's data using remote procedure calls.
    /// </summary>
    public class GrpcService : EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceBase
    {
        private readonly ILogger<GrpcService> _logger;
        private readonly Bme680Sensor _bme680Sensor;

        public GrpcService(ILogger<GrpcService> logger)
        {
            _bme680Sensor = Bme680Sensor.Instance;
            _logger = logger;
        }

        public override Task<NucuCarSensorState> GetSensorState(Empty request, ServerCallContext context)
        {
            _logger?.LogDebug($"Calling {nameof(GetSensorState)}.");
            return Task.FromResult(new NucuCarSensorState()
            {
                State = _bme680Sensor.GetState()
            });
        }

        public override Task<EnvironmentSensorMeasurement> GetSensorMeasurement(Empty request,
            ServerCallContext context)
        {
            _logger?.LogDebug($"Calling {nameof(GetSensorMeasurement)}.");
            var sensorMeasurement = _bme680Sensor.GetMeasurement();
            return Task.FromResult(sensorMeasurement);
        }
    }
}