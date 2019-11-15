using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace NucuCar.Sensors.Telemetry
{
    public class TelemetryService : IDisposable
    {
        private readonly List<ITelemetrySensor> _registeredSensors;
        private readonly IManagedMqttClient _mqttClient;
        private ILogger _logger;

        /* Singleton Instance */
        public static TelemetryService Instance { get; } = new TelemetryService();
        public string ProjectId { get; set; }
        public string Region { get; set; }
        public string RegistryId { get; set; }
        public string DeviceId { get; set; }
        public string Rs256File { get; set; }

        static TelemetryService()
        {
        }

        private string GetMqttPassword()
        {
            string jwt;
            AsymmetricCipherKeyPair keyPair;

            using (var sr = new StreamReader(Rs256File))
            {
                var pr = new PemReader(sr);
                keyPair = (AsymmetricCipherKeyPair) pr.ReadObject();
            }
            
            var rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                jwt = Jose.JWT.Encode(new Dictionary<string, object>()
                {
                    ["iat"] = DateTime.UtcNow,
                    ["exp"] = DateTime.UtcNow.AddDays(60),
                    ["aud"] = ProjectId
                }, rsa, Jose.JwsAlgorithm.RS256);
            }

            return jwt;
        }

        private TelemetryService()
        {
            _registeredSensors = new List<ITelemetrySensor>(5);

            _mqttClient = new MqttFactory().CreateManagedMqttClient();
        }

        public void Dispose()
        {
        }

        public async Task StartAsync()
        {
            _logger.LogInformation("Starting the MQTT client.");
            ManagedMqttClientOptions options; 
            try
            {
                options = new ManagedMqttClientOptionsBuilder()
                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                    .WithClientOptions(new MqttClientOptionsBuilder()
                        .WithClientId($"projects/{ProjectId}/locations/{Region}/registries/{RegistryId}/devices/{DeviceId}")
                        .WithCredentials("unused", GetMqttPassword())
                        .WithTcpServer("mqtt.googleapis.com")
                        .WithTls().Build())
                    .Build();
            }
            catch (IOException e)
            {
                _logger.LogCritical(e.Message);
                throw;
            }

            await _mqttClient.StartAsync(options);
            _logger.LogInformation("Started the MQTT client!");
        }

        public async Task PublishDataAsync(CancellationToken cancellationToken)
        {
            foreach (var sensor in _registeredSensors)
            {
                var data = sensor.GetTelemetryData();
                if (data == null)
                {
                    _logger.LogWarning($"Warning! Data for {sensor.GetIdentifier()} is null!");
                    continue;
                }
                await UploadData(data, cancellationToken);
            }
        }

        private async Task UploadData(Dictionary<string, double> data, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Stopping the MQTT client, cancellation requested.");
                await _mqttClient.StopAsync();
            }

            foreach (var entry in data)
            {
                await _mqttClient.PublishAsync(entry.Key, entry.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void RegisterSensor(ITelemetrySensor sensor)
        {
            _registeredSensors.Add(sensor);
        }

        public void UnregisterSensor(ITelemetrySensor sensor)
        {
            _registeredSensors.Remove(sensor);
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }
    }
}