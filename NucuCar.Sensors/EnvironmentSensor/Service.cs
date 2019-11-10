using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NucuCarGrpcSensors;

namespace NucuCar.Sensors.EnvironmentSensor
{
    public class Service : EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceBase
    {
        private readonly ILogger<Service> _logger;
        private readonly Sensor _sensor;

        public Service(ILogger<Service> logger)
        {
            _sensor = Sensor.Instance;
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}