using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using NucuCar.Domain.Utilities;

namespace NucuCar.Domain.Telemetry
{
    /// <summary>
    /// This class is used to publish the telemetry data to Google's Cloud Firestore.
    /// Requires the environment variable: GOOGLE_APPLICATION_CREDENTIALS to be set.
    /// See: https://cloud.google.com/docs/authentication/getting-started
    /// or Firebase > Project Settings > Service Accounts
    /// <remarks>
    ///    The connection string has the following parameters:
    ///    ProjectId (required) — The string for the Firestore project id.
    ///    CollectionName (required) — The string for the Firestore collection name.
    ///    Timeout (optional) — The number in milliseconds in which to timeout if publishing fails. Default: 10000
    /// </remarks>
    /// </summary>
    public class TelemetryPublisherFirestore : TelemetryPublisher
    {
        private readonly FirestoreDb _database;
        private readonly string _firestoreCollection;
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

            if (!options.TryGetValue("CollectionName", out _firestoreCollection))
            {
                Logger?.LogCritical(
                    $"Can't start {nameof(TelemetryPublisherFirestore)}! Malformed connection string! " +
                    $"Missing CollectionName!");
            }
            _timeout = int.Parse(options.GetValueOrDefault("Timeout", "10000"));


            _database = FirestoreDb.Create(firestoreProjectId);
            Logger?.LogInformation($"Initialized {nameof(TelemetryPublisherFirestore)}");
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var docRef = _database.Collection(_firestoreCollection).Document();
            var data = GetTelemetry();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(_timeout);
            try
            {
                await docRef.SetAsync(data, cancellationToken: cts.Token);
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
