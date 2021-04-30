# Introduction

This module will read sensors data periodically and
provide access to it via gRPC, while optionally publishing telemetry data into the cloud.

For installing see instructions from the Readme.md file located at the root directory.

# Wiring

_You may download the Fritzing diagrams from `Docs/fritzing/` in order to play with them._

Wire your sensor according to the following diagram:

![raspberry pi wiring diagram](../Docs/images/nucucar.sensors_bb.jpg)

#### BME680

Connect the BME680 sensor to the I2C bus 1 (I2C.1) of the Raspberry Pi.
The address ` 0x76` will be used to communicate with the sensor.

Make sure I2C is enabled. Use `raspi-config`.

# Configuration

Each Sensor is configured via the `appsettings.json` file. The following configuration options are
available to all sensors:

- Enabled: (bool) Enables or disables the sensor.
- Telemetry: (bool) Enables or disables telemetry reporting for the sensor.
- Grpc: (bool): Enables or disables the gRPC endpoint for the sensor.
- MeasurementInterval: (int) The interval in milliseconds in which the sensor will read data. Default: 3000

---
Besides sensor configuration, you may also configure the telemetry service. The telemetry service collects telemetry data from all sensors
and uses a telemetry publisher to publish the data.

Example config:
```
{
    "Telemetry": {
        "Publisher": "Disk",
        "ServiceEnabled": true,
        "PublishInterval": 3000,
        "ConnectionString": "FileName=nucucar.telemetry;FileExtension=txt;Separator=\n"
    },
    "EnvironmentSensor": {
        "Enabled": true,
        "Telemetry": true,
        "Grpc": true
    },
}
```

---

# Sensors

#### Environment Sensor

We use [BME680](https://www.bosch-sensortec.com/bst/products/all_products/bme680) environmental sensor from Bosh.

Sensor capabilities:

* Temperature
* Barometric Pressure
* Humidity
* VOC Gas (Gas Resistance)