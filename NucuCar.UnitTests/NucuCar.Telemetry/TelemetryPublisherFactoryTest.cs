using System;
using NucuCar.Telemetry;
using NucuCar.Telemetry.Publishers;
using Xunit;

namespace NucuCar.UnitTests.NucuCar.Telemetry
{
    public class TelemetryPublisherFactoryTest
    {
        [Fact]
        private void Test_Build_TelemetryPublisherAzure()
        {
            const string connectionString =
                "HostName=something.azure-devices.net;DeviceId=something;SharedAccessKey=test";
            var telemetryPublisher =
                PublisherFactory.CreateFromConnectionString(PublisherType.Azure, connectionString);
            Assert.IsType<Azure>(telemetryPublisher);
        }
        
        [Fact]
        private void Test_Build_TelemetryPublisherDisk()
        {
            const string connectionString =
                "Filename=test;BufferSize=4096";
            var telemetryPublisher =
                PublisherFactory.CreateFromConnectionString(PublisherType.Disk, connectionString);
            Assert.IsType<Disk>(telemetryPublisher);
        }
        
        [Fact]
        private void Test_Build_ThrowsOnInvalidType()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                PublisherFactory.CreateFromConnectionString("_1", "a=b");
            });
        }
    }
}