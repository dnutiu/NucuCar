using Microsoft.Extensions.Logging;
using Moq;
using NucuCar.Sensors;
using NucuCar.Sensors.EnvironmentSensor;
using NucuCarSensorsProto;
using Xunit;
using Xunit.Abstractions;

namespace NucuCar.UnitTests.NucuCar.Sensors.Tests.EnvironmentSensor.Tests
{
    public partial class Bme680GrpcServiceTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly Mock<ILogger<Bme680GrpcService>> _mockLogger;
        private readonly Mock<ISensor<Bme680Sensor>> _mockSensor;

        public Bme680GrpcServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _mockLogger = new Mock<ILogger<Bme680GrpcService>>();
            _mockSensor = new Mock<ISensor<Bme680Sensor>>();
            _mockSensor.Setup(ms => ms.Object).Returns(new Mock<TestBme680Sensor>().Object);
        }


        [Fact]
        public void Test_GetSensorState()
        {
            var service = new Bme680GrpcService(_mockLogger.Object, _mockSensor.Object);
            var result = service.GetSensorState(null, null).Result;

            // Default sensor state is error
            Assert.Equal(SensorStateEnum.Error, result.State);

            // Verify that the sensor get state method is called.
            _mockSensor.Verify(s => s.Object.GetState(), Times.AtLeastOnce());
            
            _mockSensor.Setup(s => s.Object.GetState()).Returns(SensorStateEnum.Initialized);
            result = service.GetSensorState(null, null).Result;
            Assert.Equal(SensorStateEnum.Initialized, result.State);
        }
        
        [Fact]
        public void Test_GetSensorMeasurement()
        {
            var service = new Bme680GrpcService(_mockLogger.Object, _mockSensor.Object);
            service.GetSensorMeasurement(null, null);
            
            // Verify that the sensor get measurement method is called.
            _mockSensor.Verify(s => s.Object.GetMeasurement(), Times.AtLeastOnce());

        }
    }
}