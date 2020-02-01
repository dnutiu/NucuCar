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
                Default = "https://localhost:8000")]
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
                default:
                {
                    throw new ArgumentException($"Invalid sensor name: ${options.SensorName}");
                }
            }
        }

        public async Task EnvironmentSensorGrpcServiceTest()
        {
            var cts = SetupCancellation();
            var client = SetupGrpc();

            while (true)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(1000);

                var reply = await client.GetStateAsync(new Empty());
                var state = reply.State;

                _logger.LogInformation("EnvironmentSensorState: " + state);
                if (state != SensorStateEnum.Initialized) continue;

                var measurementJson = await client.GetMeasurementAsync(new Empty());
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

        private EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceClient SetupGrpc()
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
            var client = new EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceClient(channel);

            return client;
        }
    }
}