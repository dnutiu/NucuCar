// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NucuCarSensorsProto;

namespace NucuCar.TestClient
{
    public class SensorsCmd
    {
        [Verb("sensors", HelpText = "Test the gRPC sensors services.")]
        public class SensorsCmdOptions
        {
            [Option('u', "url", Required = false, HelpText = "The url and port of the gRPC server.",
                Default = "https://localhost:8000")]
            public string GrpcServiceAddress { get; set; }
        }
        
        public string GrpcServiceAddress { get; set; }
        private static ILogger _logger;
        
        public static async Task RunSensorsTestCommand(SensorsCmdOptions options)
        {
            _logger = LoggerFactory.Create(builder => { builder.AddConsole(); }).CreateLogger<SensorsCmd>();
            var sensorsCommandLine = new SensorsCmd();
            sensorsCommandLine.GrpcServiceAddress = options.GrpcServiceAddress;

            await sensorsCommandLine.EnvironmentSensorGrpcServiceTest();
        }

        public async Task EnvironmentSensorGrpcServiceTest()
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
            var reply = await client.GetSensorStateAsync(new Empty());
            var state = reply.State;
            _logger.LogInformation("EnvironmentSensorState: " + state);
            if (state == SensorStateEnum.Initialized)
            {
                var measurement = await client.GetSensorMeasurementAsync(new Empty());
                _logger.LogInformation(
                    $"t: {measurement.Temperature} | h: {measurement.Humidity} | p: {measurement.Pressure}");
            }

            _logger.LogInformation("Done");
        }
    }
}