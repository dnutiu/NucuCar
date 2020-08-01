using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NucuCar.Sensors.Abstractions;
using NucuCar.Sensors.Grpc;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.Modules.Health
{
    public class CpuTempGrpcService : HealthSensorGrpcService.HealthSensorGrpcServiceBase
    {
        private readonly ILogger<CpuTempGrpcService> _logger;
        private readonly ISensor<CpuTempSensor> _sensor;
        private readonly IOptions<CpuTempConfig> _options;

        public CpuTempGrpcService(ILogger<CpuTempGrpcService> logger, ISensor<CpuTempSensor> sensor,
            IOptions<CpuTempConfig> options)
        {
            _logger = logger;
            _sensor = sensor;
            _options = options;
        }

        public override async Task<NucuCarSensorResponse> GetCpuTemperature(Empty request, ServerCallContext context)
        {
            _logger?.LogDebug($"Calling {nameof(GetCpuTemperature)}.");
            if (_options.Value.Grpc)
            {
                return await Task.FromResult(_sensor.Object.GetMeasurement());
            }

            return await Task.FromResult(Responses.GrpcIsDisabledResponse);
        }
    }
}