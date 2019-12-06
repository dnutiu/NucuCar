using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NucuCar.Sensors;
using NucuCar.Sensors.EnvironmentSensor;
using NucuCarSensorsProto;
using Xunit;

namespace NucuCar.UnitTests.NucuCar.Sensors.Tests.EnvironmentSensor.Tests
{
    public class Bme680GrpcServiceTest
    {
        private readonly Mock<ILogger<Bme680GrpcService>> _mockLogger;
        private readonly Mock<ISensor<Bme680Sensor>> _mockSensor;
        private readonly Mock<TestBme680Sensor> _mockTestSensor;

        public Bme680GrpcServiceTest()
        {
            _mockLogger = new Mock<ILogger<Bme680GrpcService>>();
            _mockSensor = new Mock<ISensor<Bme680Sensor>>();
            _mockTestSensor = new Mock<TestBme680Sensor>();
            _mockSensor.Setup(ms => ms.Object).Returns(_mockTestSensor.Object);
        }


        [Fact]
        public void Test_GetSensorState()
        {
            var service = new Bme680GrpcService(_mockLogger.Object, _mockSensor.Object);
            var result = service.GetState(null, null).Result;

            // Default sensor state is error
            Assert.Equal(SensorStateEnum.Error, result.State);

            // Verify that the sensor get state method is called.
            _mockSensor.Verify(s => s.Object.GetState(), Times.AtLeastOnce());
            
            _mockSensor.Setup(s => s.Object.GetState()).Returns(SensorStateEnum.Initialized);
            result = service.GetState(null, null).Result;
            Assert.Equal(SensorStateEnum.Initialized, result.State);
        }
        
        [Fact]
        public void Test_GetSensorMeasurement()
        {
            _mockTestSensor.Setup(s => s.GetMeasurement()).Returns(new Bme680MeasurementData());
            var service = new Bme680GrpcService(_mockLogger.Object, _mockSensor.Object);
            service.GetMeasurement(null, null);

            // Verify that the sensor get measurement method is called.
            _mockSensor.Verify(s => s.Object.GetMeasurement(), Times.AtLeastOnce());

        }
    }
}