using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NucuCar.Domain.Http
{
    /// <summary>
    /// A simple HttpClient wrapper designed to make it easier to work with web requests with media type application/json.
    /// It implements a simple retry mechanism.
    /// </summary>
    public class MinimalHttpClient : IDisposable
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

        private readonly HttpClient _httpClient;

        #endregion

        #region Constructors

        public MinimalHttpClient()
        {
            _httpClient = new HttpClient();
            maxRetries = 3;
            timeout = 10000;
            Logger = null;
        }

        public MinimalHttpClient(string baseAddress) : this()
        {
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        public MinimalHttpClient(string baseAddress, int maxRetries) : this(baseAddress)
        {
            MaxRetries = maxRetries;
        }

        #endregion

        #region Public Methods

        public void ClearAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public void Authorization(string scheme, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(scheme, token);
        }

        public void Authorization(string token)
        {
            Authorization("Bearer", token);
        }

        public async Task<HttpResponseMessage> GetAsync(string path)
        {
            var request = _makeRequest(HttpMethod.Get, path);
            return await SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostAsync(string path, Dictionary<string, object> data)
        {
            var request = _makeRequest(HttpMethod.Post, path);
            request.Content = _makeContent(data);
            return await SendAsync(request);
        }

        public async Task<HttpResponseMessage> PutAsync(string path, Dictionary<string, object> data)
        {
            var request = _makeRequest(HttpMethod.Put, path);
            request.Content = _makeContent(data);
            return await SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string path, Dictionary<string, object> data)
        {
            var request = _makeRequest(HttpMethod.Delete, path);
            request.Content = _makeContent(data);
            return await SendAsync(request);
        }

        /// <summary>
        /// Makes a request with timeout and retry support.
        /// </summary>
        /// <param name="requestMessage">The request to make.</param>
        /// <returns></returns>
        public virtual async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            var currentRetry = 0;
            HttpResponseMessage responseMessage = null;

            while (currentRetry < maxRetries)
            {
                try
                {
                    // We need a request copy because we can't send the same request multiple times.
                    var requestCopy = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                    requestCopy.Headers.Authorization = requestMessage.Headers.Authorization;
                    requestCopy.Content = requestMessage.Content;

                    responseMessage = await _sendAsync(requestCopy);
                    break;
                }
                catch (TaskCanceledException)
                {
                    Logger?.LogError("Request timeout for {Uri}!", requestMessage.RequestUri);
                }
                catch (HttpRequestException e)
                {
                    // The request failed due to an underlying issue such as network connectivity, DNS failure, 
                    //     server certificate validation or timeout.         
                    Logger?.LogError("HttpRequestException timeout for {Uri}!", requestMessage.RequestUri);
                    Logger?.LogError("{ErrorMessage}", e.Message);
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
        private StringContent _makeContent(Dictionary<string, object> data)
        {
            return new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        }

        /// <summary>
        ///  Creates a HttpRequestMessage, applies the auth header and constructs the uri.
        /// </summary>
        /// <param name="method">The HttpMethod to use</param>
        /// <param name="path">The path, whether it is relative to the base or a new one.</param>
        /// <returns></returns>
        private HttpRequestMessage _makeRequest(HttpMethod method, string path)
        {
            var uri = _httpClient.BaseAddress == null ? new Uri(path) : new Uri(_httpClient.BaseAddress, path);

            var requestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = uri
            };
            requestMessage.Headers.Authorization = _httpClient.DefaultRequestHeaders.Authorization;

            return requestMessage;
        }

        /// <summary>
        /// Makes a request which gets cancelled after Timeout.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> _sendAsync(HttpRequestMessage requestMessage)
        {
            var cts = new CancellationTokenSource();
            HttpResponseMessage response;

            // Make sure we cancel after a certain timeout.
            cts.CancelAfter(timeout);
            try
            {
                response = await _httpClient.SendAsync(requestMessage,
                    HttpCompletionOption.ResponseContentRead,
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
        /// <param name="responseMessage">The HttpResponseMessage message. <see cref="HttpResponseMessage"/></param>
        /// <returns>A JsonElement. <see cref="JsonElement"/></returns>
        public static async Task<JsonElement> GetJson(this HttpResponseMessage responseMessage)
        {
            return JsonSerializer.Deserialize<JsonElement>(await responseMessage.Content.ReadAsStringAsync());
        }
    }
}