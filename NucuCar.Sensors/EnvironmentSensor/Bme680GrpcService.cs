using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.EnvironmentSensor
{
    /// <summary>
    /// EnvironmentSensor's gRPC service.
    /// It allows reading the sensor's data using remote procedure calls.
    /// </summary>
    public class Bme680GrpcService : EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceBase
    {
        private readonly ILogger<Bme680GrpcService> _logger;
        private readonly ISensor<Bme680Sensor> _bme680Sensor;

        public Bme680GrpcService(ILogger<Bme680GrpcService> logger, ISensor<Bme680Sensor> bme680Sensor)
        {
            _bme680Sensor = bme680Sensor;
            _logger = logger;
        }

        public override Task<NucuCarSensorState> GetState(Empty request, ServerCallContext context)
        {
            _logger?.LogDebug($"Calling {nameof(GetState)}.");
            return Task.FromResult(new NucuCarSensorState()
            {
                State = _bme680Sensor.Object.GetState()
            });
        }

        public override Task<NucuCarSensorResponse> GetMeasurement(Empty request,
            ServerCallContext context)
        {
            _logger?.LogDebug($"Calling {nameof(GetMeasurement)}.");
            var sensorMeasurement = _bme680Sensor.Object.GetMeasurement();
            var jsonResponse = JsonConvert.SerializeObject(sensorMeasurement);
            return Task.FromResult(new NucuCarSensorResponse()
            {
                State = _bme680Sensor.Object.GetState(),
                JsonData = jsonResponse
            });
        }
    }
}