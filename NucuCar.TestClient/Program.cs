using System;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using NucuCarSensorsProto;

namespace NucuCar.TestClient
{
    class Program
    {
        /* Warning! Before issuing a gRPC call the dev certificate must be trusted or you'll get:
         * Detail="Error starting gRPC call: The SSL connection could not be established, see inner exception."
         * See: https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.0&tabs=visual-studio#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos
         */
        private static HttpClient _httpClient;

        // ReSharper disable once ArrangeTypeMemberModifiers
        static async Task Main(string[] args)
        {
            // Used to allow gRPC calls over unsecured HTTP.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // Allow untrusted certificates.
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            _httpClient = new HttpClient(httpClientHandler);
            
            await EnvironmentSensorGrpcServiceTest();
        }

        private static async Task EnvironmentSensorGrpcServiceTest()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:8000",
                new GrpcChannelOptions {HttpClient = _httpClient});
            var client = new EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceClient(channel);
            var reply = await client.GetSensorStateAsync(new Empty());
            var state = reply.State;
            Console.WriteLine("EnviromentSensorState: " + state);
            if (state == SensorStateEnum.Initialized)
            {
                var measurement = await client.GetSensorMeasurementAsync(new Empty());
                Console.WriteLine(
                    $"t: {measurement.Temperature} | h: {measurement.Humidity} | p: {measurement.Pressure}");
            }
        }
    }
}