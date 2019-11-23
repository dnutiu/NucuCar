using CommandLine;

namespace NucuCar.TestClient
{
    internal class Program
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        static int Main(string[] args)
        {
            return Parser.Default
                .ParseArguments<SensorsCmd.SensorsCmdOptions, AzureTelemetryPublishCmd.AzureTelemetryPublishOptions,
                    AzureTelemetryReaderCmd.AzureTelemetryReaderOpts>(args)
                .MapResult(
                    (SensorsCmd.SensorsCmdOptions opts) =>
                    {
                        SensorsCmd.RunSensorsTestCommand(opts).GetAwaiter().GetResult();
                        return 0;
                    },
                    (AzureTelemetryPublishCmd.AzureTelemetryPublishOptions opts) =>
                    {
                        AzureTelemetryPublishCmd.RunAzurePublisherTelemetryTest(opts).GetAwaiter().GetResult();
                        return 0;
                    },
                    (AzureTelemetryReaderCmd.AzureTelemetryReaderOpts opts) =>
                    {
                        AzureTelemetryReaderCmd.RunAzureTelemetryReaderTest(opts).GetAwaiter().GetResult();
                        return 0;
                    },
                    errs => 1);
        }
    }
}