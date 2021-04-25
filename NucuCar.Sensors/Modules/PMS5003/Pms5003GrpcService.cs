using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;
using NucuCar.Sensors.Grpc;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.Modules.PMS5003
{
    public class Pms5003GrpcService : Pms5003SensorGrpcService.Pms5003SensorGrpcServiceBase
    {
        private readonly ILogger<Pms5003GrpcService> _logger;
        private readonly IOptions<Pms5003Config> _options;
        private readonly ISensor<Pms5003Sensor> _pms5003Sensor;

        public Pms5003GrpcService(ILogger<Pms5003GrpcService> logger, ISensor<Pms5003Sensor> pms5003Sensor, IOptions<Pms5003Config> options)
        {
            _pms5003Sensor = pms5003Sensor;
            _logger = logger;
            _options = options;
        }

        public override async Task<NucuCarSensorResponse> GetMeasurement(Empty request, ServerCallContext context)
        {
            _logger?.LogDebug($"Calling {nameof(GetMeasurement)}.");
            if (_options.Value.Grpc)
            {
                return await Task.FromResult(_pms5003Sensor.Object.GetMeasurement());
            }

            return await Task.FromResult(Responses.GrpcIsDisabledResponse);
        }
    }
}