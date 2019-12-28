using System;
using NucuCar.Domain.Telemetry;
using Xunit;

namespace NucuCar.UnitTests.NucuCar.Domain.Tests.Telemetry
{
    public class TelemetryPublisherFactoryTest
    {
        [Fact]
        private void Test_Build_TelemetryPublisherAzure()
        {
            const string connectionString =
                "HostName=something.azure-devices.net;DeviceId=something;SharedAccessKey=test";
            var telemetryPublisher =
                TelemetryPublisherFactory.CreateFromConnectionString(TelemetryPublisherType.Azure, connectionString);
            Assert.IsType<TelemetryPublisherAzure>(telemetryPublisher);
        }
        
        [Fact]
        private void Test_Build_TelemetryPublisherDisk()
        {
            const string connectionString =
                "Filename=test;BufferSize=4096";
            var telemetryPublisher =
                TelemetryPublisherFactory.CreateFromConnectionString(TelemetryPublisherType.Disk, connectionString);
            Assert.IsType<TelemetryPublisherDisk>(telemetryPublisher);
        }

        [Fact]
        private void Test_Build_ThrowsOnInvalidType()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TelemetryPublisherFactory.CreateFromConnectionString("_1", "a=b");
            });
        }
    }
}