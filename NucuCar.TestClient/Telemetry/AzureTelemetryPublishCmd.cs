// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NucuCar.Domain.Telemetry;

namespace NucuCar.TestClient.Telemetry
{
    public class AzureTelemetryPublishCmd
    {
        [Verb("azure-telemetry-publish", HelpText = "Test the publishing telemetry using Microsoft Azure IoT Hub.")]
        public class AzureTelemetryPublishOptions
        {
            [Option('c', "connectionString", Required = true,
                HelpText = "The publisher's connection string. Get it from the Device.")]
            public string PublisherConnectionString { get; set; }

            [Option('m', "message", Required = true, HelpText = "The message to publish")]
            public string PublisherJsonMessage { get; set; }
        }

        private class DummyTelemeter : ITelemeter
        {
            private readonly Dictionary<string, object> _dummyTelemeterData;

            public DummyTelemeter(Dictionary<string, object> dummyData)
            {
                _dummyTelemeterData = dummyData;
            }

            public string GetIdentifier()
            {
                return "DummyTelemeter";
            }

            public Dictionary<string, object> GetTelemetryData()
            {
                return _dummyTelemeterData;
            }
        }

        public static async Task RunAzurePublisherTelemetryTest(AzureTelemetryPublishOptions opts)
        {
            var logger = LoggerFactory.Create(builder => { builder.AddConsole(); })
                .CreateLogger<AzureTelemetryPublishCmd>();

            var telemetryPublisher = TelemetryPublisherFactory.Create(TelemetryPublisherType.Azure,
                opts.PublisherConnectionString, "NucuCar.TestClient", logger);
            
            var anonymousTelemeter =
                new DummyTelemeter(
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(opts.PublisherJsonMessage));


            logger.LogInformation($"Publishing message: {opts.PublisherJsonMessage}");
            telemetryPublisher.RegisterTelemeter(anonymousTelemeter);
            await telemetryPublisher.PublishAsync(CancellationToken.None);
        }
    }
}