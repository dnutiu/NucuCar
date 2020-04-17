using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NucuCar.Domain.Telemetry;
using NucuCar.Domain.Utilities;

namespace NucuCar.Telemetry
{
    /// <summary>
    /// This class is used to publish the telemetry data to Google's Cloud Firestore.
    /// Requires the environment variable: GOOGLE_APPLICATION_CREDENTIALS to be set.
    /// See: https://cloud.google.com/docs/authentication/getting-started
    /// or Firebase > Project Settings > Service Accounts (Authentication is not implemented!)
    /// <remarks>
    ///    The connection string has the following parameters:
    ///    ProjectId (required) — The string for the Firestore project id.
    ///    CollectionName (required) — The string for the Firestore collection name.
    ///    WebApiKey (optional) — The web api key of the firebase project.
    ///    WebApiEmail (optional) — An email to use when requesting id tokens.
    ///    WebApiPassword (optional) — The password to use when requesting id tokens.
    ///    Timeout (optional) — The number in milliseconds in which to timeout if publishing fails. Default: 10000
    /// </remarks>
    /// </summary>
    public class TelemetryPublisherFirestore : TelemetryPublisher
    {
        private readonly HttpClient _httpClient;
        private readonly int _timeout;

        private string _idToken;

        // Variables used for authentication
        private readonly string _webEmail;
        private readonly string _webPassword;
        private readonly string _webApiKey;

        public TelemetryPublisherFirestore(TelemetryPublisherBuilderOptions opts) : base(opts)
        {
            var options = ConnectionStringParser.Parse(opts.ConnectionString);
            if (!options.TryGetValue("ProjectId", out var firestoreProjectId))
            {
                Logger?.LogCritical(
                    $"Can't start {nameof(TelemetryPublisherFirestore)}! Malformed connection string! " +
                    $"Missing ProjectId!");
            }

            if (!options.TryGetValue("CollectionName", out var firestoreCollection))
            {
                Logger?.LogCritical(
                    $"Can't start {nameof(TelemetryPublisherFirestore)}! Malformed connection string! " +
                    $"Missing CollectionName!");
            }

            _timeout = int.Parse(options.GetValueOrDefault("Timeout", "10000") ?? "10000");
            _webApiKey = options.GetValueOrDefault("WebApiKey", null);
            _webEmail = options.GetValueOrDefault("WebApiEmail", null);
            _webPassword = options.GetValueOrDefault("WebApiPassword", null);

            var requestUrl = $"https://firestore.googleapis.com/v1/projects/{firestoreProjectId}/" +
                             $"databases/(default)/documents/{firestoreCollection}/";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(requestUrl);
            Logger?.LogInformation($"Initialized {nameof(TelemetryPublisherFirestore)}");
            Logger?.LogInformation($"ProjectId: {firestoreProjectId}; CollectionName: {firestoreCollection}.");
        }

        private async Task SetupAuthenticationHeaders()
        {
            // Make request
            var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_webApiKey}";
            var data = new Dictionary<string, object>()
            {
                ["email"] = _webEmail,
                ["password"] = _webPassword,
                ["returnSecureToken"] = true
            };
            // Handle response & setup headers
            var cts = new CancellationTokenSource();
            try
            {
                cts.CancelAfter(_timeout);
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var responseMessage = await _httpClient.PostAsync(requestUrl, content, cts.Token);
                var responseJson =
                    JsonConvert.DeserializeObject<dynamic>(await responseMessage.Content.ReadAsStringAsync());
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    _idToken = responseJson.idToken;
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _idToken);
                }
                else
                {
                    throw new HttpRequestException(responseJson.message ?? "unknown");
                }
            }
            catch (TaskCanceledException e)
            {
                Logger.LogWarning($"FireStore authenticate: Timeout or cancellation occured. Message {e.Message}.\n");
            }
            catch (HttpRequestException e)
            {
                Logger?.LogError($"Failed to authenticate!\n{e.GetType().FullName}: {e.Message}");
                throw;
            }
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var cts = new CancellationTokenSource();
            try
            {
                cts.CancelAfter(_timeout);
                var data = FirebaseRestTranslator.Translator.Translate(null, GetTelemetry());
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var responseMessage = await _httpClient.PostAsync("", content, cts.Token);
                var responseJson =
                    JsonConvert.DeserializeObject<dynamic>(await responseMessage.Content.ReadAsStringAsync());

                switch (responseMessage.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Logger?.LogInformation("Published data to Firestore!");
                        break;
                    case HttpStatusCode.Forbidden:
                        Logger.LogWarning("Failed to publish telemetry! Forbidden!");
                        await SetupAuthenticationHeaders();
                        break;
                    default:
                        throw new HttpRequestException(responseJson.message ?? "unknown error");
                }
            }
            catch (TaskCanceledException e)
            {
                Logger.LogWarning(
                    $"Firestore publish telemetry: Timeout or cancellation occured. Message {e.Message}.\n");
            }
            catch (HttpRequestException e)
            {
                Logger?.LogError($"Failed to publish telemetry data!\n{e.GetType().FullName}: {e.Message}");
                throw;
            }
            finally
            {
                cts.Dispose();
            }
        }

        public override void Dispose()
        {
        }
    }
}