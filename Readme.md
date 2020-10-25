# WIP

## Solution Information

An attempt to create a remote controlled toy car. 
Powered by Raspberry Pi and .Net Core 3.1.

https://docs.microsoft.com/en-us/dotnet/core/install/

[![CircleCI](https://circleci.com/gh/dnutiu/NucuCar.svg?style=svg)](https://circleci.com/gh/dnutiu/NucuCar)

### NucuCar.Domain

Holds common classes and interfaces that are required by the projects. 

It provides all the types that are generated via protocol buffers.

### NucuCar.Common

Contains implementations for common logic, contains wrappers.
Usually utility classes and methods that are not necessarily tied to this solution
and can be reused.

### NucuCar.Telemetry

Holds concrete implementation for telemetry publishers and workers.

### NucuCar.Sensors

Manages all the car sensors. For more info see the readme file located in the project directory.

### NucuCar.TestClient

Command line utility to play around with the car functionality. You can use it to remotely read data from the sensors via gRPC methods and to publish and read telemetry data.

---

### Building and Running.

To build the project and target the Raspberry Pi you can use the following command:

```$xslt
dotnet build --runtime linux-arm -p:PublishSingleFile=true --configuration Release
```
---

To run the project you can take advantage of `docker-compsose` and run it with the following commands:

```bash
git clone https://github.com/dnutiu/NucuCar.git
cd NucuCar
docker-compose up
```