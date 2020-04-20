using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NucuCar.Domain.Telemetry;
using NucuCar.Telemetry;
using Xunit;
using HttpClient = NucuCar.Common.HttpClient;

namespace NucuCar.UnitTests.NucuCar.Telemetry.Tests
{
    /// <summary>
    /// Class used to test the TelemetryPublisherFirestore by mocking the GetTelemetry method and HttpClient field.
    /// </summary>
    internal class MockTelemetryPublisherFirestore : TelemetryPublisherFirestore
    {
        private Dictionary<string, object> _mockData;

        public MockTelemetryPublisherFirestore(TelemetryPublisherBuilderOptions opts) : base(opts)
        {
            _mockData = new Dictionary<string, object>();
        }

        public void SetHttpClient(HttpClient client)
        {
            HttpClient = client;
        }

        public void SetMockData(Dictionary<string, object> data)
        {
            _mockData = data;
        }

        protected override Dictionary<string, object> GetTelemetry()
        {
            return _mockData;
        }
    }

    public class TelemetryPublisherFirestoreTest
    {
        [Fact]
        private void Test_Construct_BadProjectId()
        {
            // Setup
            var opts = new TelemetryPublisherBuilderOptions()
            {
                ConnectionString = "ProjectIdBAD=test;CollectionName=test"
            };

            // Run & Assert
            Assert.Throws<ArgumentException>(() => { new MockTelemetryPublisherFirestore(opts); });
        }

        [Fact]
        private void Test_Construct_BadCollectiontName()
        {
            // Setup
            var opts = new TelemetryPublisherBuilderOptions()
            {
                ConnectionString = "ProjectId=test;CollectionNameBAD=test"
            };

            // Run & Assert
            Assert.Throws<ArgumentException>(() => { new MockTelemetryPublisherFirestore(opts); });
        }

        [Fact]
        private async Task Test_PublishAsync_OK()
        {
            // Setup
            var opts = new TelemetryPublisherBuilderOptions()
            {
                ConnectionString = "ProjectId=test;CollectionName=test"
            };
            var publisher = new MockTelemetryPublisherFirestore(opts);
            var mockHttpClient = new Mock<HttpClient>("http://testing.com");
            mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            publisher.SetHttpClient(mockHttpClient.Object);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});

            // Run
            await publisher.PublishAsync(CancellationToken.None);

            // Assert
            var expectedContent = "{\"fields\":{\"testData\":{\"integerValue\":1}}}";
            mockHttpClient.Verify(
                m => m.SendAsync(
                    It.Is<HttpRequestMessage>(
                        request => request.Method.Equals(HttpMethod.Post))));
            mockHttpClient.Verify(
                m => m.SendAsync(
                    It.Is<HttpRequestMessage>(
                        request => request.RequestUri.Equals(new Uri("http://testing.com")))));
            mockHttpClient.Verify(
                m => m.SendAsync(
                    It.Is<HttpRequestMessage>(
                        request => request.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                            .Equals(expectedContent))));
        }

        [Fact]
        private async Task Test_PublishAsync_Cancel()
        {
            // Setup
            var opts = new TelemetryPublisherBuilderOptions()
            {
                ConnectionString = "ProjectId=test;CollectionName=test"
            };
            var publisher = new MockTelemetryPublisherFirestore(opts);
            var mockHttpClient = new Mock<HttpClient>("http://testing.com");
            mockHttpClient.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            publisher.SetHttpClient(mockHttpClient.Object);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Run
            await publisher.PublishAsync(cts.Token);

            // Assert
            mockHttpClient.Verify(m => m.SendAsync(It.IsAny<HttpRequestMessage>()), Times.Never());
        }

        [Fact]
        private async Task Test_PublishAsync_Authorization_OK()
        {
            // Setup
            var opts = new TelemetryPublisherBuilderOptions()
            {
                ConnectionString = "ProjectId=test;CollectionName=test"
            };
            var publisher = new MockTelemetryPublisherFirestore(opts);
            var mockHttpClient = new Mock<HttpClient>("http://testing.com");
            mockHttpClient.SetupSequence(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)))
                .Returns(Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                        {Content = new StringContent("{\"idToken\":\"testauthtoken\"}")}
                ))
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));


            publisher.SetHttpClient(mockHttpClient.Object);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});

            // Run
            await publisher.PublishAsync(CancellationToken.None);

            // Assert
            // Can't verify because moq doesn't support that, damn C#.
        }
    }
}