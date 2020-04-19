using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using sNetHttp = System.Net.Http;
using sNetHttpHeaders = System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace NucuCar.Common
{
    /// <summary>
    /// A simple HttpClient wrapper designed to make it easier to work with web requests with media type application/json.
    /// It implements a simple retry mechanism.
    /// </summary>
    public class HttpClient : IDisposable
    {
        #region Fields

        public ILogger Logger { get; set; }

        public int MaxRetries
        {
            get => maxRetries;
            set
            {
                if (value < 0 || value > 10)
                {
                    throw new ArgumentOutOfRangeException($"Maximum retries allowed value is between 0 and 10!");
                }

                maxRetries = value;
            }
        }

        public int Timeout
        {
            get => timeout;
            set
            {
                if (value < 0 || value > 10000)
                {
                    throw new ArgumentOutOfRangeException($"Timeout allowed value is between 0 and 10000!");
                }

                timeout = value;
            }
        }

        // ReSharper disable InconsistentNaming
        protected int maxRetries;

        protected int timeout;
        // ReSharper restore InconsistentNaming

        private readonly sNetHttp.HttpClient _httpClient;

        #endregion

        #region Constructors

        public HttpClient()
        {
            _httpClient = new sNetHttp.HttpClient();
            maxRetries = 3;
            timeout = 10000;
            Logger = null;
        }

        public HttpClient(string baseAddress) : this()
        {
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        public HttpClient(string baseAddress, int maxRetries) : this(baseAddress)
        {
            MaxRetries = maxRetries;
        }

        #endregion


        #region Public Methods

        public void Authorization(string scheme, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new sNetHttpHeaders.AuthenticationHeaderValue(scheme, token);
        }

        public void Authorization(string token)
        {
            Authorization("Bearer", token);
        }

        public async Task<sNetHttp.HttpResponseMessage> GetAsync(string path)
        {
            var request = _makeRequest(sNetHttp.HttpMethod.Get, path);
            return await SendAsync(request);
        }

        public async Task<sNetHttp.HttpResponseMessage> PostAsync(string path, Dictionary<string, object> data)
        {
            var request = _makeRequest(sNetHttp.HttpMethod.Post, path);
            request.Content = _makeContent(data);
            return await SendAsync(request);
        }

        public async Task<sNetHttp.HttpResponseMessage> PutAsync(string path, Dictionary<string, object> data)
        {
            var request = _makeRequest(sNetHttp.HttpMethod.Put, path);
            request.Content = _makeContent(data);
            return await SendAsync(request);
        }

        public async Task<sNetHttp.HttpResponseMessage> DeleteAsync(string path, Dictionary<string, object> data)
        {
            var request = _makeRequest(sNetHttp.HttpMethod.Delete, path);
            request.Content = _makeContent(data);
            return await SendAsync(request);
        }

        /// <summary>
        /// Makes a request with timeout and retry support.
        /// </summary>
        /// <param name="requestMessage">The request to make.</param>
        /// <returns></returns>
        public virtual async Task<sNetHttp.HttpResponseMessage> SendAsync(sNetHttp.HttpRequestMessage requestMessage)
        {
            var currentRetry = 0;
            sNetHttp.HttpResponseMessage responseMessage = null;

            while (currentRetry < maxRetries)
            {
                try
                {
                    responseMessage = await _sendAsync(requestMessage);
                    break;
                }
                catch (TaskCanceledException)
                {
                    Logger?.LogError($"Request timeout for {requestMessage.RequestUri}!");
                }
                catch (sNetHttp.HttpRequestException e)
                {
                    // The request failed due to an underlying issue such as network connectivity, DNS failure, 
                    //     server certificate validation or timeout.         
                    Logger?.LogError($"HttpRequestException timeout for {requestMessage.RequestUri}!");
                    Logger?.LogError($"{e.Message}");
                }
                finally
                {
                    currentRetry += 1;
                }
            }

            return responseMessage;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region NonPublic Methods

        /// <summary>
        /// Creates a StringContent with media type of application.json and encodes it with UTF8.
        /// </summary>
        /// <param name="data">A dictionary representing JSON data.</param>
        /// <returns></returns>
        private sNetHttp.StringContent _makeContent(Dictionary<string, object> data)
        {
            return new sNetHttp.StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        }

        /// <summary>
        ///  Creates a HttpRequestMessage, applies the auth header and constructs the uri.
        /// </summary>
        /// <param name="method">The HttpMethod to use</param>
        /// <param name="path">The path, whether it is relative to the base or a new one.</param>
        /// <returns></returns>
        private sNetHttp.HttpRequestMessage _makeRequest(sNetHttp.HttpMethod method, string path)
        {
            var requestMessage = new sNetHttp.HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(_httpClient.BaseAddress, path)
            };
            requestMessage.Headers.Authorization = _httpClient.DefaultRequestHeaders.Authorization;

            return requestMessage;
        }

        /// <summary>
        /// Makes a request which gets cancelled after Timeout.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private async Task<sNetHttp.HttpResponseMessage> _sendAsync(sNetHttp.HttpRequestMessage requestMessage)
        {
            var cts = new CancellationTokenSource();
            sNetHttp.HttpResponseMessage response;

            // Make sure we cancel after a certain timeout.
            cts.CancelAfter(timeout);
            try
            {
                response = await _httpClient.SendAsync(requestMessage,
                    sNetHttp.HttpCompletionOption.ResponseContentRead,
                    cts.Token);
            }
            finally
            {
                cts.Dispose();
            }

            return response;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
        }

        #endregion
    }

    /// <summary>
    ///  HttpClientResponseMessageExtension provides extensions methods for the HttpResponseMessage class. 
    /// </summary>
    public static class HttpResponseMessageExtension
    {
        /// <summary>
        /// Extension used to deserialize the body of a HttpResponseMessage into Json.
        /// </summary>
        /// <param name="responseMessage">The HttpResponseMessage message. <see cref="sNetHttp.HttpResponseMessage"/></param>
        /// <returns>A JsonElement. <see cref="JsonElement"/></returns>
        public static async Task<JsonElement> GetJson(this sNetHttp.HttpResponseMessage responseMessage)
        {
            return JsonSerializer.Deserialize<JsonElement>(await responseMessage.Content.ReadAsStringAsync());
        }
    }
}