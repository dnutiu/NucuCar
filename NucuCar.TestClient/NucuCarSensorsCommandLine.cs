using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommandLine;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using NucuCarSensorsProto;

namespace NucuCar.TestClient
{
    [Verb("sensors", HelpText = "Test the gRPC sensors services.")]
    public class NucuCarSensorsCommandLineOptions
    {
        [Option('u', "url", Required = false, HelpText = "The url and port of the gRPC server.",
            Default = "https://localhost:8000")]
        public string GrpcServiceAddress { get; set; }
    }

    public class NucuCarSensorsCommandLine
    {
        public string GrpcServiceAddress { get; set; }

        public static async Task RunSensorsTestCommand(NucuCarSensorsCommandLineOptions options)
        {
            var sensorsCommandLine = new NucuCarSensorsCommandLine();
            sensorsCommandLine.GrpcServiceAddress = options.GrpcServiceAddress;

            await sensorsCommandLine.EnvironmentSensorGrpcServiceTest();
        }

        public async Task EnvironmentSensorGrpcServiceTest()
        {
            // Used to allow gRPC calls over unsecured HTTP.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // Allow untrusted certificates.
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);


            var channel = GrpcChannel.ForAddress(GrpcServiceAddress,
                new GrpcChannelOptions {HttpClient = httpClient});
            var client = new EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceClient(channel);
            var reply = await client.GetSensorStateAsync(new Empty());
            var state = reply.State;
            Console.WriteLine("EnvironmentSensorState: " + state);
            if (state == SensorStateEnum.Initialized)
            {
                var measurement = await client.GetSensorMeasurementAsync(new Empty());
                Console.WriteLine(
                    $"t: {measurement.Temperature} | h: {measurement.Humidity} | p: {measurement.Pressure}");
            }

            Console.WriteLine("Done");
        }
    }
}