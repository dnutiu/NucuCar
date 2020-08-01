using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HttpClient = NucuCar.Domain.HttpClient;

namespace NucuCar.UnitTests.NucuCar.Common.Tests
{
    public class MockHttpClient : Domain.HttpClient
    {
        public List<HttpRequestMessage> SendAsyncArgCalls;
        public List<HttpResponseMessage> SendAsyncResponses;

        private int _sendAsyncCallCounter;

        public MockHttpClient(string baseAddress) : base(baseAddress)
        {
            _sendAsyncCallCounter = 0;
            SendAsyncArgCalls = new List<HttpRequestMessage>();
            SendAsyncResponses = new List<HttpResponseMessage>();
        }
        
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            SendAsyncArgCalls.Add(requestMessage);
            var response = SendAsyncResponses[_sendAsyncCallCounter];
            _sendAsyncCallCounter += 1;
            return Task.FromResult(response);
        }
    }
}