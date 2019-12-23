# WIP

## Solution Information

An attempt to create a remote controlled toy car. 
Powered by Raspberry Pi and .Net Core 3.0.

[![Board Status](https://dev.azure.com/dnutiu-pub/7cbb8fd5-fa56-4314-b57d-920730d3084d/f3d592ae-fe9f-4aca-805d-6878896da467/_apis/work/boardbadge/d748b115-c743-4367-be78-efa3f448e1e8?columnOptions=1)](https://dev.azure.com/dnutiu-pub/7cbb8fd5-fa56-4314-b57d-920730d3084d/_boards/board/t/f3d592ae-fe9f-4aca-805d-6878896da467/Microsoft.RequirementCategory/)

### NucuCar.Domain

Holds common classes and interfaces that are required by the other projects. 

It provides all the types that are generated via protocol buffers.

### NucuCar.Sensors

Manages all the car sensors. For more info see the readme file located in the project directory.

### NucuCar.TestClient

Command line utility to play around with the car functionality. You can use it to remotely read data from the sensors via gRPC methods and to publish and read telemetry data.

---

### Building and Running.

To build the project and target the Raspberry Pi you can use the following command:

```$xslt
dotnet build --runtime linux-arm -p:PublishSingleFile=true
```
---