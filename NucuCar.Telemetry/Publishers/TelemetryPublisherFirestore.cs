using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Http;
using NucuCar.Domain.Utilities;
using NucuCar.Telemetry.Abstractions;
using HttpClient = NucuCar.Domain.Http.HttpClient;

namespace NucuCar.Telemetry.Publishers
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
        protected HttpClient HttpClient;

        private string _idToken;
        private DateTime _authorizationExpiryTime;

        // Variables used for authentication
        private readonly string _webEmail;
        private readonly string _webPassword;
        private readonly string _webApiKey;

        public TelemetryPublisherFirestore(TelemetryPublisherOptions opts) : base(opts)
        {
            // Parse Options
            var options = ConnectionStringParser.Parse(opts.ConnectionString);
            if (!options.TryGetValue("ProjectId", out var firestoreProjectId))
            {
                Logger?.LogCritical(
                    $"Can't start {nameof(TelemetryPublisherFirestore)}! Malformed connection string! " +
                    $"Missing ProjectId!");
                throw new ArgumentException("Malformed connection string!");
            }

            if (!options.TryGetValue("CollectionName", out var firestoreCollection))
            {
                Logger?.LogCritical(
                    $"Can't start {nameof(TelemetryPublisherFirestore)}! Malformed connection string! " +
                    $"Missing CollectionName!");
                throw new ArgumentException("Malformed connection string!");
            }

            var timeout = int.Parse(options.GetValueOrDefault("Timeout", "10000") ?? "10000");
            _webApiKey = options.GetValueOrDefault("WebApiKey", null);
            _webEmail = options.GetValueOrDefault("WebApiEmail", null);
            _webPassword = options.GetValueOrDefault("WebApiPassword", null);

            // Setup HttpClient
            var requestUrl = $"https://firestore.googleapis.com/v1/projects/{firestoreProjectId}/" +
                             $"databases/(default)/documents/{firestoreCollection}/";
            HttpClient = new HttpClient(requestUrl) {Timeout = timeout, Logger = Logger};
            Logger?.LogInformation($"Initialized {nameof(TelemetryPublisherFirestore)}");
            Logger?.LogInformation($"ProjectId: {firestoreProjectId}; CollectionName: {firestoreCollection}.");
        }

        private async Task SetupAuthorization()
        {
            // https://cloud.google.com/identity-platform/docs/use-rest-api#section-sign-in-email-password
            var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_webApiKey}";
            var data = new Dictionary<string, object>()
            {
                ["email"] = _webEmail,
                ["password"] = _webPassword,
                ["returnSecureToken"] = true
            };

            var response = await HttpClient.PostAsync(requestUrl, data);

            if (response?.StatusCode == HttpStatusCode.OK)
            {
                var jsonContent = await response.GetJson();
                _idToken = jsonContent.GetProperty("idToken").ToString();
                // Setup next expire.
                var expiresIn = double.Parse(jsonContent.GetProperty("expiresIn").ToString());
                _authorizationExpiryTime = DateTime.UtcNow.AddSeconds(expiresIn);
                HttpClient.Authorization(_idToken);
            }
            else
            {
                Logger?.LogError($"Firestore authentication request failed! {response?.StatusCode}!");
                Logger?.LogDebug($"{response?.Content}");
            }
        }

        private async Task CheckAndSetupAuthorization()
        {
            // If there are no credentials or partial credentials supplies there must be no authorization.
            if (_webApiKey == null || _webEmail == null || _webPassword == null)
            {
                return;
            }

            // Check if the token is about to expire in the next 5 minutes.
            if (DateTime.UtcNow.AddMinutes(5) < _authorizationExpiryTime)
            {
                return;
            }

            await SetupAuthorization();
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var data = FirebaseRestTranslator.Translator.Translate(null, GetTelemetry());

            HttpResponseMessage responseMessage = null;
            try
            {
                await CheckAndSetupAuthorization();
                responseMessage = await HttpClient.PostAsync("", data);
            }
            // ArgumentException occurs during json serialization errors.
            catch (ArgumentException e)
            {
                Logger?.LogWarning(e.Message);
            }


            switch (responseMessage?.StatusCode)
            {
                case HttpStatusCode.OK:
                    Logger?.LogInformation("Published data to Firestore!");
                    break;
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.Unauthorized:
                {
                    Logger?.LogError($"Failed to publish telemetry data! {responseMessage.StatusCode}. Retrying...");
                    await SetupAuthorization();
                    responseMessage = await HttpClient.PostAsync("", data);
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