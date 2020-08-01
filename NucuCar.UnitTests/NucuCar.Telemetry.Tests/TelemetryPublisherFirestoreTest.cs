using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NucuCar.Domain.Telemetry;
using NucuCar.Telemetry;
using NucuCar.UnitTests.NucuCar.Common.Tests;
using Xunit;
using HttpClient = NucuCar.Domain.HttpClient;

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
            var mockHttpClient = new MockHttpClient("http://testing.com");
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK));
            publisher.SetHttpClient(mockHttpClient);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});

            // Run
            await publisher.PublishAsync(CancellationToken.None);

            // Assert
            var request = mockHttpClient.SendAsyncArgCalls[0];
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("http://testing.com"), request.RequestUri);
            Assert.Equal("{\"fields\":{\"testData\":{\"integerValue\":1}}}",
                request.Content.ReadAsStringAsync().GetAwaiter().GetResult());
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
            var mockHttpClient = new MockHttpClient("http://testing.com");
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK));

            publisher.SetHttpClient(mockHttpClient);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Run
            await publisher.PublishAsync(cts.Token);

            // Assert
            Assert.Equal(mockHttpClient.SendAsyncArgCalls.Count, 0);
        }

        [Fact]
        private async Task Test_PublishAsync_Authorization_OK()
        {
            // Setup
            var opts = new TelemetryPublisherBuilderOptions()
            {
                ConnectionString =
                    "ProjectId=test;CollectionName=test;WebApiKey=TAPIKEY;WebApiEmail=t@emai.com;WebApiPassword=tpass"
            };
            var publisher = new MockTelemetryPublisherFirestore(opts);
            var mockHttpClient = new MockHttpClient("http://testing.com");
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.Forbidden));
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"idToken\":\"testauthtoken\"}")
            });
            mockHttpClient.SendAsyncResponses.Add(new HttpResponseMessage(HttpStatusCode.OK));


            publisher.SetHttpClient(mockHttpClient);
            publisher.SetMockData(new Dictionary<string, object> {["testData"] = 1});

            // Run
            await publisher.PublishAsync(CancellationToken.None);

            // Assert
            Assert.Equal(3, mockHttpClient.SendAsyncArgCalls.Count);

            // 1st request - auth denied
            Assert.Equal(HttpMethod.Post, mockHttpClient.SendAsyncArgCalls[0].Method);
            Assert.Equal(new Uri("http://testing.com"), mockHttpClient.SendAsyncArgCalls[0].RequestUri);
            Assert.Equal("{\"fields\":{\"testData\":{\"integerValue\":1}}}",
                mockHttpClient.SendAsyncArgCalls[0].Content.ReadAsStringAsync().GetAwaiter().GetResult());

            // 2st request - authorizing
            Assert.Equal(HttpMethod.Post, mockHttpClient.SendAsyncArgCalls[1].Method);
            Assert.Equal(new Uri("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=TAPIKEY"),
                mockHttpClient.SendAsyncArgCalls[1].RequestUri);
            Assert.Equal("{\"email\":\"t@emai.com\",\"password\":\"tpass\",\"returnSecureToken\":true}",
                mockHttpClient.SendAsyncArgCalls[1].Content.ReadAsStringAsync().GetAwaiter().GetResult());


            // 3st request with authorization
            Assert.Equal(HttpMethod.Post, mockHttpClient.SendAsyncArgCalls[2].Method);
            Assert.Equal(new Uri("http://testing.com"), mockHttpClient.SendAsyncArgCalls[2].RequestUri);
            Assert.Equal("{\"fields\":{\"testData\":{\"integerValue\":1}}}",
                mockHttpClient.SendAsyncArgCalls[2].Content.ReadAsStringAsync().GetAwaiter().GetResult());
            Assert.Equal(new AuthenticationHeaderValue("Bearer", "testauthtoken"),
                mockHttpClient.SendAsyncArgCalls[2].Headers.Authorization);
        }
    }
}