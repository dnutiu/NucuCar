using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NucuCar.Core.Http
{
    public class MockMinimalHttpClient : MinimalHttpClient
    {
        public readonly List<HttpRequestMessage> SendAsyncArgCalls;
        public readonly List<HttpResponseMessage> SendAsyncResponses;

        private int _sendAsyncCallCounter;

        public MockMinimalHttpClient(string baseAddress) : base(baseAddress)
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