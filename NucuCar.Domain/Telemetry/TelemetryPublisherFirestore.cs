using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NucuCar.Domain.Utilities;

namespace NucuCar.Domain.Telemetry
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
    ///    Timeout (optional) — The number in milliseconds in which to timeout if publishing fails. Default: 10000
    /// </remarks>
    /// </summary>
    public class TelemetryPublisherFirestore : TelemetryPublisher
    {
        private readonly HttpClient _httpClient;
        private readonly int _timeout;

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

            _timeout = int.Parse(options.GetValueOrDefault("Timeout", "10000"));

            var requestUrl = $"https://firestore.googleapis.com/v1/projects/{firestoreProjectId}/" +
                             $"databases/(default)/documents/{firestoreCollection}/";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(requestUrl);
            Logger?.LogInformation($"Initialized {nameof(TelemetryPublisherFirestore)}");
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            
            var cts = new CancellationTokenSource();
            cts.CancelAfter(_timeout);
            try
            {
                var data = FirebaseRestTranslator.Translator.Translate(null, GetTelemetry());
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("", content, cts.Token);
                Logger?.LogInformation("Published data to Firestore!");
            }
            catch (Exception e)
            {
                Logger?.LogError($"Failed to publish telemetry data!\n{e.GetType().FullName}: {e.Message}");
                throw;
            }
        }

        public override void Dispose()
        {
        }
    }
}