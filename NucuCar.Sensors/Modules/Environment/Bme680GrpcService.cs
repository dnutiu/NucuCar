using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;
using NucuCar.Sensors.Grpc;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.Modules.Environment
{
    /// <summary>
    /// EnvironmentSensor's gRPC service.
    /// It allows reading the sensor's data using remote procedure calls.
    /// </summary>
    public class Bme680GrpcService : EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceBase
    {
        private readonly ILogger<Bme680GrpcService> _logger;
        private readonly IOptions<Bme680Config> _options;
        private readonly ISensor<Bme680Sensor> _bme680Sensor;

        public Bme680GrpcService(ILogger<Bme680GrpcService> logger, ISensor<Bme680Sensor> bme680Sensor, IOptions<Bme680Config> options)
        {
            _bme680Sensor = bme680Sensor;
            _logger = logger;
            _options = options;
        }

        public override Task<NucuCarSensorResponse> GetMeasurement(Empty request,
            ServerCallContext context)
        {
            _logger?.LogDebug($"Calling {nameof(GetMeasurement)}.");
            if (_options.Value.Grpc)
            {
                return Task.FromResult(_bme680Sensor.Object.GetMeasurement());
            }

            return Task.FromResult(Responses.GrpcIsDisabledResponse);
        }
    }
}