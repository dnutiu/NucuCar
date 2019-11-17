using System;
using System.Collections.Generic;
using CommandLine;
using static NucuCar.TestClient.SensorsCommandLine;

namespace NucuCar.TestClient
{
    internal class Program
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        static void Main(string[] args)
        {
            // TODO: Add test for telemetry publisher.
            // TODO: Add test for telemetry reader.
            // Sample: https://github.com/Azure-Samples/azure-iot-samples-csharp/blob/master/iot-hub/Quickstarts/read-d2c-messages/ReadDeviceToCloudMessages.cs
            Parser.Default.ParseArguments<SensorsCommandLineOptions>(args)
                .WithParsed(opts => { RunSensorsTestCommand(opts).GetAwaiter().GetResult(); })
                .WithNotParsed(HandleParseError);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var e in errs)
            {
                Console.WriteLine($"Argument parse error: {e}");
            }
        }
    }
}