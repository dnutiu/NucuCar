using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NucuCar.Core.Http;
using NucuCar.Telemetry;
using NucuCar.Telemetry.Publishers;
using Xunit;

namespace NucuCar.UnitTests.NucuCar.Telemetry
{
    /// <summary>
    /// Class used to test the TelemetryPublisherFirestore by mocking the GetTelemetry method and HttpClient field.
    /// </summary>
    internal class MockTelemetryPublisherFirestore : TelemetryPublisherFirestore
    {
        private Dictionary<string, object> _mockData;

        public MockTelemetryPublisherFirestore(TelemetryPublisherOptions opts) : base(opts)
        {
            _mockData = new Dictionary<string, object>();
        }

        public void SetHttpClient(MinimalHttpClient client)
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
            var opts = new TelemetryPublisherOptions()
            {
                ConnectionString = "ProjectIdBAD=test;CollectionName=test"
            };

            // Run & Assert
            Assert.Throws<ArgumentException>(() => { new MockTelemetryPublisherFirestore(opts); });
        }

        [Fact]
        private void Test_Construct_BadCollectionName()
        {
            // Setup
            var opts = new TelemetryPublisherOptions()
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
            var opts = new TelemetryPublisherOptions()
            {
                ConnectionString = "ProjectId=test;CollectionName=test;WebApiKey=TAPIKEY;WebApiEmail=t@emai.com;WebApiPassword=tpass"
            };
            var publisher = new MockTelemetryPublisherFirestore(opts);
            var mockHttpClient = new MockMinimalHttpClient("http://testing.com");
            var authResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {Content = new StringContent("{\"idToken\": \"1\",\"expiresIn\": \"3600\"}")};
            mockHttpClient.SendAsyncResponses.Add(authResponse);
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK));
            publisher.SetHttpClient(mockHttpClient);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});

            // Run
            await publisher.PublishAsync(CancellationToken.None);

            // Assert
            var request = mockHttpClient.SendAsyncArgCalls[1];
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("http://testing.com"), request.RequestUri);
            Assert.Equal("{\"fields\":{\"testData\":{\"integerValue\":1}}}",
                request.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }

        [Fact]
        private async Task Test_PublishAsync_InvalidJson()
        {
            // Setup
            var opts = new TelemetryPublisherOptions()
            {
                ConnectionString = "ProjectId=test;CollectionName=test;WebApiKey=TAPIKEY;WebApiEmail=t@emai.com;WebApiPassword=tpass"
            };
            var publisher = new MockTelemetryPublisherFirestore(opts);
            var mockHttpClient = new MockMinimalHttpClient("http://testing.com");
            var authResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {Content = new StringContent("{\"idToken\": \"1\",\"expiresIn\": \"3600\"}")};
            mockHttpClient.SendAsyncResponses.Add(authResponse);
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK));
            publisher.SetHttpClient(mockHttpClient);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = double.PositiveInfinity});

            // Run
            await publisher.PublishAsync(CancellationToken.None);

            // Assert only auth request made.
            Assert.Single(mockHttpClient.SendAsyncArgCalls);
        }

        [Fact]
        private async Task Test_PublishAsync_Cancel()
        {
            // Setup
            var opts = new TelemetryPublisherOptions()
            {
                ConnectionString = "ProjectId=test;CollectionName=test"
            };
            var publisher = new MockTelemetryPublisherFirestore(opts);
            var mockHttpClient = new MockMinimalHttpClient("http://testing.com");
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK));

            publisher.SetHttpClient(mockHttpClient);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Run
            await publisher.PublishAsync(cts.Token);

            // Assert
            Assert.Empty(mockHttpClient.SendAsyncArgCalls);
        }

        [Fact]
        private async Task Test_PublishAsync_Authorization_Refresh()
        {
            // Setup
            var opts = new TelemetryPublisherOptions()
            {
                ConnectionString =
                    "ProjectId=test;CollectionName=test;WebApiKey=TAPIKEY;WebApiEmail=t@emai.com;WebApiPassword=tpass"
            };
            var publisher = new MockTelemetryPublisherFirestore(opts);
            var mockHttpClient = new MockMinimalHttpClient("http://testing.com");

            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK)
                {Content = new StringContent("{\"idToken\": \"1\",\"expiresIn\": \"0\"}")});
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK));
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK)
                {Content = new StringContent("{\"idToken\": \"1\",\"expiresIn\": \"3600\"}")});
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK));

            publisher.SetHttpClient(mockHttpClient);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});

            // Run
            await publisher.PublishAsync(CancellationToken.None);
            await publisher.PublishAsync(CancellationToken.None);

            // Assert
            Assert.Equal(4, mockHttpClient.SendAsyncArgCalls.Count);

            // 1st request auth
            Assert.Equal(HttpMethod.Post, mockHttpClient.SendAsyncArgCalls[0].Method);
            Assert.Equal("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=TAPIKEY",
                mockHttpClient.SendAsyncArgCalls[0].RequestUri.ToString());

            // 2st request payload
            Assert.Equal(HttpMethod.Post, mockHttpClient.SendAsyncArgCalls[1].Method);

            // 3rd request auth
            Assert.Equal(HttpMethod.Post, mockHttpClient.SendAsyncArgCalls[2].Method);
            Assert.Equal("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=TAPIKEY",
                mockHttpClient.SendAsyncArgCalls[2].RequestUri.ToString());

            // 4th request payload
            Assert.Equal(HttpMethod.Post, mockHttpClient.SendAsyncArgCalls[1].Method);
        }
    }
}