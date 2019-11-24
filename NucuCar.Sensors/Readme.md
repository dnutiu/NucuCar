The service will gather sensor data and provide access to it via gRPC.

## Installation How-To

1. Build the module and target the Raspberry Pi.
2. Copy the files over to the Raspberry Pi.
3. Modify `appsettings.json` to fit your needs.
4. Install the hardware.

#### BME680

Connect the BME680 sensor to the I2C bus 1 (I2C.1) of the Raspberry Pi. 
The address ` 0x76` will be used to communicate with the sensor.
 
 I2C Data: https://pinout.xyz/pinout/pin3_gpio2
 
 I2C Clock: https://pinout.xyz/pinout/pin5_gpio3
 
 3.3V Power: https://pinout.xyz/pinout/pin1_3v3_power
 
 Ground: https://pinout.xyz/pinout/ground


5\. Run the application.

### Environment Sensor

Worker service for the [BME680](https://www.bosch-sensortec.com/bst/products/all_products/bme680) enviromental sensor from Bosh.

Sensor capabilities:

* Temperature
* Barometric Pressure
* Humidity
* VOC Gas (Currently not implemented in binding)