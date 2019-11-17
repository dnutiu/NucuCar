## Protos

The files from Protos are compiled by the gRPC compiler and provide data 
structures that are shared across the project.

## Telemetry

It provides usesful classes and abstractions for implementing telemetry
in individual components. All telemetry publishers use the MQTT protocol unless specified
otherwise.

### Azure Telemetry

#### Publisher

You can use cloud telemetry for free via Azure-IoT-Hub.
You still need a backend application that will process the messages.

You will need to create a:
- IoT Hub
- IoT Device

Then navigate to your device and grab the primary key, you will need it to create
a connection string of the form:

`HostName=YOUR_IOT_HUB_NAME.azure-devices.net;DeviceId=YOUR_DEVICE_NAME;SharedAccessKey=PRIMARY_OR_SECONDARY_KEY`

The connection string can be passed to the application via `appsettings.json` or command line arguments or environment variables:
```
dotnet run --Telemetry:AzureIotHubConnectionString=CONNECTION_STRING
export Telemetry:AzureIotHubConnectionString=CONNECTION_STRING
```
