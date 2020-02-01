using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NucuCarSensorsProto;

namespace NucuCar.Sensors.Health
{
    public class CpuTempGrpcService : HealthSensorGrpcService.HealthSensorGrpcServiceBase
    {
        private readonly ILogger<CpuTempGrpcService> _logger;
        private readonly ISensor<CpuTempSensor> _sensor;


        public CpuTempGrpcService(ILogger<CpuTempGrpcService> logger, ISensor<CpuTempSensor> sensor)
        {
            _logger = logger;
            _sensor = sensor;
        }
        
        public override Task<NucuCarSensorResponse> GetCpuTemperature(Empty request, ServerCallContext context)
        {
            _logger?.LogDebug($"Calling {nameof(GetCpuTemperature)}.");
            return Task.FromResult(_sensor.Object.GetMeasurement());
        }
    }
}