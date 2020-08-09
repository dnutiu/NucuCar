// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NucuCarSensorsProto;

namespace NucuCar.TestClient.Sensors
{
    public class SensorsCmd
    {
        [Verb("sensors", HelpText = "Test the gRPC sensors services.")]
        public class SensorsCmdOptions
        {
            [Option('u', "url", Required = false, HelpText = "The url and port of the gRPC server.",
                Default = "http://localhost:8000")]
            public string GrpcServiceAddress { get; set; }

            [Option('s', "sensor", Required = false, HelpText = "The sensor name you'd like to test.",
                Default = "environment")]
            public string SensorName { get; set; }
        }

        public string GrpcServiceAddress { get; set; }
        private static ILogger _logger;

        public static async Task RunSensorsTestCommand(SensorsCmdOptions options)
        {
            _logger = LoggerFactory.Create(builder => { builder.AddConsole(); }).CreateLogger<SensorsCmd>();
            var sensorsCommandLine = new SensorsCmd();
            sensorsCommandLine.GrpcServiceAddress = options.GrpcServiceAddress;

            switch (options.SensorName)
            {
                case "environment":
                {
                    await sensorsCommandLine.EnvironmentSensorGrpcServiceTest();
                    break;
                }
                case "health":
                {
                    await sensorsCommandLine.HealthSensorGrpcServiceTest();
                    break;
                }
                default:
                {
                    throw new ArgumentException($"Invalid sensor name: ${options.SensorName}");
                }
            }
        }

        private async Task HealthSensorGrpcServiceTest()
        {
            var cts = SetupCancellation();
            var channel = SetupGrpc();
            var client = new HealthSensorGrpcService.HealthSensorGrpcServiceClient(channel);


            while (true)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(1000, cts.Token);

                var measurementJson = await client.GetCpuTemperatureAsync(new Empty());
                _logger.LogInformation("State: " + measurementJson.State);
                _logger.LogInformation("CpuTemperature: " + measurementJson.JsonData);
            }
        }

        public async Task EnvironmentSensorGrpcServiceTest()
        {
            var cts = SetupCancellation();
            var channel = SetupGrpc();
            var client = new EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceClient(channel);

            while (true)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(1000, cts.Token);
                
                var measurementJson = await client.GetMeasurementAsync(new Empty());
                _logger.LogInformation("State " +  measurementJson.State);
                _logger.LogInformation(measurementJson.JsonData);
            }
        }

        private static CancellationTokenSource SetupCancellation()
        {
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Shutting down...");
            };
            return cts;
        }

        private GrpcChannel SetupGrpc()
        {
            // Used to allow gRPC calls over unsecured HTTP.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // Allow untrusted certificates.
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(httpClientHandler);


            var channel = GrpcChannel.ForAddress(GrpcServiceAddress,
                new GrpcChannelOptions {HttpClient = httpClient});
            return channel;
        }
    }
}