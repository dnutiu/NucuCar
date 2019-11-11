using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NucuCarGrpcSensors;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public class GrpcService : EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceBase
    {
        private readonly ILogger<GrpcService> _logger;
        private readonly Sensor _sensor;

        public GrpcService(ILogger<GrpcService> logger)
        {
            _sensor = Sensor.Instance;
            _logger = logger;
        }

        public override Task<NucuCarSensorState> GetSensorState(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new NucuCarSensorState()
            {
                State = _sensor.GetState()
            });
        }

        public override Task<EnvironmentSensorMeasurement> GetSensorMeasurement(Empty request, ServerCallContext context)
        {
            var sensorMeasurement = _sensor.GetMeasurement();
            return Task.FromResult(sensorMeasurement);
        }
    }
}