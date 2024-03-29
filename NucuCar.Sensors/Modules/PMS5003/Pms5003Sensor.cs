﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NucuCar.Sensors.Abstractions;
using PMS5003;
using PMS5003.Exceptions;

namespace NucuCar.Sensors.Modules.PMS5003
{
    public class Pms5003Sensor : GenericTelemeterSensor, IDisposable, ISensor<Pms5003Sensor>
    {
        private readonly ILoggerFactory _loggerFactory;
        private Pms5003 _pms5003;
        private Pms5003Data _pms5003Data;

        public Pms5003Sensor()
        {
        }

        public Pms5003Sensor(ILoggerFactory loggingFactory, IOptions<Pms5003Config> options)
        {
            CurrentState = SensorStateEnum.Uninitialized;
            Logger = loggingFactory.CreateLogger(nameof(Pms5003Sensor));
            if (!options.Value.Enabled)
            {
                Logger?.LogDebug("Pms5003 Sensor is disabled!");
                CurrentState = SensorStateEnum.Disabled;
            }
            
            _loggerFactory = loggingFactory;
            TelemetryEnabled = options.Value.Telemetry;
        }

        public override void Initialize()
        {
            if (_pms5003 != null)
            {
                CurrentState = SensorStateEnum.Initialized;
                return;
            }

            try
            {
                Pms5003.Logger = new Logger<Pms5003>(_loggerFactory);
                _pms5003 = new Pms5003(23, 24);
                _pms5003.Reset();
                CurrentState = SensorStateEnum.Initialized;
                Logger.LogInformation("Pms5003Sensor has initialized");
            }
            catch (ArgumentException e)
            {
                Logger.LogError("{Message}", e.Message);
                CurrentState = SensorStateEnum.Error;
            }
        }

        public override Task TakeMeasurementAsync()
        {
            if (CurrentState == SensorStateEnum.Disabled)
            {
                throw new InvalidOperationException("Can't take measurement on disabled sensor!");
            }

            try
            {
                _pms5003.WakeUp();
                _pms5003Data = _pms5003.ReadData();
                Logger?.LogDebug("{Message}", _pms5003Data.ToString());
                CurrentState = SensorStateEnum.Initialized;
            }
            catch (ReadFailedException e)
            {
                Logger?.LogError("{Message}", e.Message);
                CurrentState = SensorStateEnum.Error;
                _pms5003.Reset();
            }
            finally
            {
                _pms5003.Sleep();
            }

            return Task.CompletedTask;
        }

        public override SensorResponse GetMeasurement()
        {
            return new SensorResponse()
            {
                SensorId = GetIdentifier(),
                State = GetState(),
                Data = new List<SensorMeasurement>
                {
                    new SensorMeasurement("Pm1Atmospheric", "?", _pms5003Data.Pm1Atmospheric),
                    new SensorMeasurement("Pm1Standard", "?", _pms5003Data.Pm1Standard),
                    new SensorMeasurement("Pm10Atmospheric", "?", _pms5003Data.Pm10Atmospheric),
                    new SensorMeasurement("Pm10Standard", "?", _pms5003Data.Pm10Standard),
                    new SensorMeasurement("Pm2Dot5Atmospheric", "?", _pms5003Data.Pm2Dot5Atmospheric),
                    new SensorMeasurement("Pm2Dot5Standard", "?", _pms5003Data.Pm2Dot5Standard),
                    new SensorMeasurement("ParticlesDiameterBeyond0Dot3", "?",
                        _pms5003Data.ParticlesDiameterBeyond0Dot3),
                    new SensorMeasurement("ParticlesDiameterBeyond0Dot5", "?",
                        _pms5003Data.ParticlesDiameterBeyond0Dot5),
                    new SensorMeasurement("ParticlesDiameterBeyond1Dot0", "?",
                        _pms5003Data.ParticlesDiameterBeyond1Dot0),
                    new SensorMeasurement("ParticlesDiameterBeyond2Dot5", "?",
                        _pms5003Data.ParticlesDiameterBeyond2Dot5),
                    new SensorMeasurement("ParticlesDiameterBeyond5Dot0", "?",
                        _pms5003Data.ParticlesDiameterBeyond5Dot0),
                    new SensorMeasurement("ParticlesDiameterBeyond10Dot0", "?",
                        _pms5003Data.ParticlesDiameterBeyond10Dot0),
                }
            };
        }

        public override SensorStateEnum GetState()
        {
            return CurrentState;
        }

        public override string GetIdentifier()
        {
            return "Pms5003";
        }

        public override JObject GetTelemetryJson()
        {
            JObject returnValue = null;
            if (_pms5003Data != null && TelemetryEnabled)
            {
                // The telemetry handled by FirebaseRestTranslator wants the values to be int or double.
                returnValue = new JObject()
                {
                    ["sensor_state"] = GetState().ToString(),
                    ["Pm1Atmospheric"] = _pms5003Data.Pm1Atmospheric,
                    ["Pm1Standard"] = _pms5003Data.Pm1Standard,
                    ["Pm10Atmospheric"] = _pms5003Data.Pm10Atmospheric,
                    ["Pm10Standard"] = _pms5003Data.Pm10Standard,
                    ["Pm2Dot5Atmospheric"] = _pms5003Data.Pm2Dot5Atmospheric,
                    ["Pm2Dot5Standard"] = _pms5003Data.Pm2Dot5Standard,
                    ["ParticlesDiameterBeyond0Dot3"] = _pms5003Data.ParticlesDiameterBeyond0Dot3,
                    ["ParticlesDiameterBeyond0Dot5"] = _pms5003Data.ParticlesDiameterBeyond0Dot5,
                    ["ParticlesDiameterBeyond1Dot0"] = _pms5003Data.ParticlesDiameterBeyond1Dot0,
                    ["ParticlesDiameterBeyond2Dot5"] = _pms5003Data.ParticlesDiameterBeyond2Dot5,
                    ["ParticlesDiameterBeyond5Dot0"] = _pms5003Data.ParticlesDiameterBeyond5Dot0,
                    ["ParticlesDiameterBeyond10Dot0"] = _pms5003Data.ParticlesDiameterBeyond10Dot0,
                };
            }

            return returnValue;
        }

        public override bool IsTelemetryEnabled()
        {
            return TelemetryEnabled;
        }

        public void Dispose()
        {
            _pms5003 = null;
        }

        public Pms5003Sensor Object => this;
    }
}