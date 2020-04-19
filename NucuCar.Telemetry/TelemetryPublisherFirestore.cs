using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NucuCar.Common;
using NucuCar.Common.Utilities;
using NucuCar.Domain.Telemetry;

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

        private string _idToken;

        // Variables used for authentication
        private readonly string _webEmail;
        private readonly string _webPassword;
        private readonly string _webApiKey;

        public TelemetryPublisherFirestore(TelemetryPublisherBuilderOptions opts) : base(opts)
        {
            // Parse Options
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

            var timeout = int.Parse(options.GetValueOrDefault("Timeout", "10000") ?? "10000");
            _webApiKey = options.GetValueOrDefault("WebApiKey", null);
            _webEmail = options.GetValueOrDefault("WebApiEmail", null);
            _webPassword = options.GetValueOrDefault("WebApiPassword", null);

            // Setup HttpClient
            var requestUrl = $"https://firestore.googleapis.com/v1/projects/{firestoreProjectId}/" +
                             $"databases/(default)/documents/{firestoreCollection}/";
            _httpClient = new HttpClient(requestUrl) {Timeout = timeout, Logger = Logger};
            Logger?.LogInformation($"Initialized {nameof(TelemetryPublisherFirestore)}");
            Logger?.LogInformation($"ProjectId: {firestoreProjectId}; CollectionName: {firestoreCollection}.");
        }

        private async Task SetupAuthorization()
        {
            // Make request
            var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_webApiKey}";
            var data = new Dictionary<string, object>()
            {
                ["email"] = _webEmail,
                ["password"] = _webPassword,
                ["returnSecureToken"] = true
            };

            var response = await _httpClient.PostAsync(requestUrl, data);

            if (response?.StatusCode == HttpStatusCode.OK)
            {
                var jsonContent = await response.GetJson();
                _idToken = jsonContent.GetProperty("idToken").ToString();
                _httpClient.Authorization(_idToken);
            }
            else
            {
                Logger?.LogError($"Firestore authentication request failed! {response?.StatusCode}!");
                Logger?.LogDebug($"{response?.Content}");
            }
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var data = FirebaseRestTranslator.Translator.Translate(null, GetTelemetry());
            var responseMessage = await _httpClient.PostAsync("", data);

            switch (responseMessage?.StatusCode)
            {
                case HttpStatusCode.OK:
                    Logger?.LogInformation("Published data to Firestore!");
                    break;
                case HttpStatusCode.Forbidden:
                {
                    Logger?.LogError($"Failed to publish telemetry data! {responseMessage.StatusCode}. Retrying...");
                    await SetupAuthorization();
                    responseMessage = await _httpClient.PostAsync("", data);
                    if (responseMessage != null && responseMessage.IsSuccessStatusCode)
                    {
                        Logger?.LogInformation("Published data to Firestore on retry!");
                    }
                    else
                    {
                        Logger?.LogError($"Failed to publish telemetry data! {responseMessage?.StatusCode}");
                    }

                    break;
                }
                default:
                    Logger?.LogError($"Failed to publish telemetry data! {responseMessage?.StatusCode}");
                    break;
            }
        }

        public override void Dispose()
        {
        }
    }
}