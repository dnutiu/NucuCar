using System.Collections.Generic;
using Newtonsoft.Json;
using NucuCar.Domain.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace NucuCar.UnitTests.NucuCar.Domain.Tests.Utilities
{
    /*
       {
         "source":"NucuCar.Sensors",
         "timestamp":"2019-12-01T23:26:13.5537227+02:00",
         "data":[
            {
               "sensor_state":2,
               "temperature":32.65558333857916,
               "humidity":100.0,
               "pressure":62228.49565168124,
               "voc":0.0,
               "_id":"Bme680-Sensor"
            },
            {
               "sensor_state":2,
               "cpu_temperature":48.849998474121094,
               "_id":"CpuTemperature"
            }
         ]
       }
     */
    public class FirebaseRestTranslatorTests
    {
        public FirebaseRestTranslatorTests(ITestOutputHelper testOutputHelper)
        {
        }

        private Dictionary<string, object> getBasicTelemetryData()
        {
            var basicTelemetryDict = new Dictionary<string, object>
            {
                ["source"] = "NucuCar.Sensors",
                ["timestamp"] = "2019-12-01T23:26:13.5537227+02:00"
            };
            var data = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
                {
                    ["sensor_state"] = 2,
                    ["cpu_temperature"] = 48.849998474121094,
                    ["_id"] = "CpuTemperature",
                },
                new Dictionary<string, object>()
                {
                    ["sensor_state"] = 2,
                    ["temperature"] = 32.65,
                    ["humidity"] = 100.0,
                    ["pressure"] = 62228.49,
                    ["voc"] = 0.0,
                    ["_id"] = "Bme680-Sensor"
                }
            };
            basicTelemetryDict["data"] = data;
            return basicTelemetryDict;
        }

        [Fact]
        public void Test_FireBaseTranslator_Parse()
        {
            var expectedJson =
                "{\"name\":\"Test\",\"fields\":{\"source\":{\"stringValue\":\"NucuCar.Sensors\"},\"timestamp\":{\"stringValue\":\"2019-12-01T23:26:13.5537227+02:00\"},\"data\":{\"arrayValue\":{\"values\":[{\"mapValue\":{\"fields\":{\"sensor_state\":{\"integerValue\":2},\"cpu_temperature\":{\"doubleValue\":48.849998474121094},\"_id\":{\"stringValue\":\"CpuTemperature\"}}}},{\"mapValue\":{\"fields\":{\"sensor_state\":{\"integerValue\":2},\"temperature\":{\"doubleValue\":32.65},\"humidity\":{\"doubleValue\":100.0},\"pressure\":{\"doubleValue\":62228.49},\"voc\":{\"doubleValue\":0.0},\"_id\":{\"stringValue\":\"Bme680-Sensor\"}}}}]}}}}";
            var basicTelemetryData = getBasicTelemetryData();
            var result = FirebaseRestTranslator.Translate("Test", basicTelemetryData);
            var json = JsonConvert.SerializeObject(result);
            Assert.Equal(expectedJson, json);
        }
    }
}