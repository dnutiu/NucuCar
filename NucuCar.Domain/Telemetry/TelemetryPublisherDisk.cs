using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NucuCar.Domain.Utilities;

namespace NucuCar.Domain.Telemetry
{
    /// <summary>
    /// The TelemetryPublisherDisk is used to publish telemetry data to a file residing on the disk.
    /// </summary>
    public class TelemetryPublisherDisk : TelemetryPublisher
    {
        private readonly FileStream _fileStream;
        private int _charactersWritten;

        /// <summary>
        /// Constructs an instance of <see cref="TelemetryPublisherDisk"/>.
        /// <remarks>
        ///    The connection string must contain the following options:
        ///     Filename (required) - The path of the filename in which to log telemetry data.
        ///     FileExtension (optional) - The extension of the filename. Default txt
        ///     BufferSize (optional) - The buffer size for the async writer. Default: 4096
        /// </remarks>
        /// </summary>
        /// <param name="opts"></param>
        public TelemetryPublisherDisk(TelemetryPublisherBuilderOptions opts) : base(opts)
        {
            var connectionStringParams = ConnectionStringParser.Parse(opts.ConnectionString);
            var fileName = connectionStringParams.GetValueOrDefault("FileName");
            var fileExtension = connectionStringParams.GetValueOrDefault("FileExtension", "txt");
            var bufferSize = connectionStringParams.GetValueOrDefault("BufferSize", "4096");

            _fileStream = new FileStream(NormalizeFilename(fileName, fileExtension), FileMode.Append, FileAccess.Write,
                FileShare.Read, int.Parse(bufferSize), true);
            Logger?.LogDebug("Initialized the TelemetryPublisherDisk!");
        }

        public override async Task PublishAsync(CancellationToken cancellationToken)
        {
            var data = GetTelemetry();
            var messageString = JsonConvert.SerializeObject(data);
            Logger?.LogDebug($"Telemetry message: {messageString}");
            var encodedText = Encoding.Unicode.GetBytes(messageString);

            await _fileStream.WriteAsync(encodedText, _charactersWritten,
                encodedText.Length, cancellationToken);
            _charactersWritten += encodedText.Length;
            await _fileStream.FlushAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _fileStream.Close();
        }

        private static string NormalizeFilename(string filename, string extension)
        {
            var date = DateTime.UtcNow;
            return $"{filename}-{date.Year}-{date.Month}-{date.Day}_{date.ToFileTimeUtc()}.{extension}";
        }
    }
}