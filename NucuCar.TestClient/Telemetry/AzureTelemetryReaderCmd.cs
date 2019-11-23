// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NucuCar.TestClient.Telemetry
{
    public class AzureTelemetryReaderCmd
    {
        private static ILogger _logger;

        [Verb("azure-telemetry-read", HelpText = "Test reading the telemetry using Microsoft Azure's IoT Hub.")]
        public class AzureTelemetryReaderOpts
        {
            [Option('c', "connectionString", Required = true,
                HelpText = "The connection string for the event hub. Get it from 'Build-in endpoints'")]
            public string EventHubConnectionString { get; set; }
        }

        private static EventHubClient _eventHubClient;

        public static async Task RunAzureTelemetryReaderTest(AzureTelemetryReaderOpts opts)
        {
            _logger = LoggerFactory.Create(builder => { builder.AddConsole(); })
                .CreateLogger<AzureTelemetryReaderCmd>();

            _eventHubClient = EventHubClient.CreateFromConnectionString(opts.EventHubConnectionString);

            var runtimeInfo = await _eventHubClient.GetRuntimeInformationAsync();
            var d2CPartitions = runtimeInfo.PartitionIds;

            _logger.LogInformation("Starting reading messages from the Azure IoT Hub... Press Ctrl-C to cancel");
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                _logger.LogInformation("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (string partition in d2CPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }

            // Wait for all the PartitionReceivers to finsih.
            Task.WaitAll(tasks.ToArray());
        }

        // Asynchronously create a PartitionReceiver for a partition and then start 
        // reading any messages sent from the simulated client.
        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            // Create the receiver using the default consumer group.
            // For the purposes of this sample, read only messages sent since 
            // the time the receiver is created. Typically, you don't want to skip any messages.
            var eventHubReceiver =
                _eventHubClient.CreateReceiver("$Default", partition, EventPosition.FromEnqueuedTime(DateTime.Now));
            _logger.LogInformation("Create receiver on partition: " + partition);
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }
                _logger.LogInformation("Listening for messages on: " + partition);
                // Check for EventData - this methods times out if there is nothing to retrieve.
                var events = await eventHubReceiver.ReceiveAsync(100);

                // If there is data in the batch, process it.
                if (events == null) continue;

                foreach (var eventData in events)
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array);
                    _logger.LogInformation($"Message received on partition {partition}:");
                    _logger.LogInformation($"Data:  {data}:");
                    _logger.LogInformation("Application properties (set by device):");

                    var sb = new StringBuilder();
                    foreach (var (key, value) in eventData.Properties)
                    {
                        sb.Append($"{key}: {value} \r\n");
                        _logger.LogInformation(sb.ToString());
                    }

                    sb.Clear();
                    _logger.LogInformation("System properties (set by IoT Hub):");
                    foreach (var (key, value) in eventData.SystemProperties)
                    {
                        sb.Append($"{key}: {value},");
                    }

                    _logger.LogInformation(sb.ToString());
                }
            }
        }
    }
}