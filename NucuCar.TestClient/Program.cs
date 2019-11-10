using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using NucuCarGrpcSensors;

namespace NucuCar.TestClient
{
    class Program
    {
        /* Warning! Before issuing a gRPC call the dev certificate must be trusted or you'll get:
         * Detail="Error starting gRPC call: The SSL connection could not be established, see inner exception."
         * See: https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.0&tabs=visual-studio#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos
         */
        static async Task Main(string[] args)
        {
            // Used to allow gRPC calls over unsecured HTTP.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // Allow untrusted certificates.
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);


            // The port number(5001) must match the port of the gRPC server.
            var channel = GrpcChannel.ForAddress("https://localhost:8000",
                new GrpcChannelOptions {HttpClient = httpClient});
            var client = new EnvironmentSensorGrpcService.EnvironmentSensorGrpcServiceClient(channel);
            var reply = await client.SayHelloAsync(
                new HelloRequest {Name = "GreeterClient"});
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}