using System;
using System.Collections.Generic;
using CommandLine;
using static NucuCar.TestClient.NucuCarSensorsCommandLine;

namespace NucuCar.TestClient
{
    class Program
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<NucuCarSensorsCommandLineOptions>(args)
                .WithParsed(opts => { RunSensorsTestCommand(opts).GetAwaiter().GetResult(); })
                .WithNotParsed(HandleParseError);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var e in errs)
            {
                Console.WriteLine("Argument parse error:", e);
            }
        }
    }
}