using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NucuCar.Sensors;
using NucuCar.Sensors.Abstractions;
using NucuCar.Sensors.Modules.Environment;
using NucuCarSensorsProto;
using Xunit;

namespace NucuCar.UnitTests.NucuCar.Sensors.Tests.EnvironmentSensor.Tests
{
    public class Bme680GrpcServiceTest
    {
        private readonly Mock<ILogger<Bme680GrpcService>> _mockLogger;
        private readonly Mock<ISensor<Bme680Sensor>> _mockSensor;
        private readonly Mock<IOptions<Bme680Config>> _mockOptions;
        private readonly Mock<TestBme680Sensor> _mockTestSensor;

        public Bme680GrpcServiceTest()
        {
            _mockLogger = new Mock<ILogger<Bme680GrpcService>>();
            _mockSensor = new Mock<ISensor<Bme680Sensor>>();
            _mockOptions = new Mock<IOptions<Bme680Config>>();
            _mockTestSensor = new Mock<TestBme680Sensor>();

            _mockOptions.Setup(mo => mo.Value).Returns(new Bme680Config()
            {
                Grpc = true,
                Telemetry = true,
                Enabled = true
            });
            _mockSensor.Setup(ms => ms.Object).Returns(_mockTestSensor.Object);
        }


        [Fact]
        public void Test_GetSensorMeasurement()
        {
            _mockTestSensor.Setup(s => s.GetMeasurement()).Returns(new NucuCarSensorResponse());
            var service = new Bme680GrpcService(_mockLogger.Object, _mockSensor.Object, _mockOptions.Object);
            service.GetMeasurement(null, null);

            // Verify that the sensor get measurement method is called.
            _mockSensor.Verify(s => s.Object.GetMeasurement(), Times.AtLeastOnce());
        }

        [Fact]
        public void Test_GetSensorMeasurement_Disabled()
        {
            _mockTestSensor.Setup(s => s.GetMeasurement()).Returns(new NucuCarSensorResponse());
            var options = new Mock<IOptions<Bme680Config>>();
            options.Setup(o => o.Value).Returns(new Bme680Config
            {
                Enabled = true,
                Telemetry = true,
                Grpc = false
            });

            var service = new Bme680GrpcService(_mockLogger.Object, _mockSensor.Object, options.Object);
            var result = service.GetMeasurement(null, null);

            // Verify that the sensor get measurement method is not called.
            _mockSensor.Verify(s => s.Object.GetMeasurement(), Times.Never());
            Assert.Equal(SensorStateEnum.GrpcDisabled, result.Result.State);
        }
    }
}