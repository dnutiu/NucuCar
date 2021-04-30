# Telemetry

This package provides useful classes and abstractions for implementing telemetry
in individual components.

The telemetry service is used by other packages in order to implement telemetry reporting. It may be configured via the `appsettings.json` file or environment variables.

# Configuration

The following configurations is available for the Telemetry service:

- Publisher: (string) The telemetry publisher. i.e: `Azure`, `Disk`, `Firestore`, `Console`.
- ServiceEnabled: (bool) Enables or disables the telemetry collection service.
- PublishInterval: (int) The interval in which to publish the telemetry data.
- ConnectionString: (string) Connection string for the telemetry publisher.

# Telemetry Publishers

Telemetry publishers are used by the telemetry service in order to publish data.

## Azure Telemetry

### Publisher

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
dotnet run --Telemetry:ConnectionString=CONNECTION_STRING
export Telemetry:ConnectionString=CONNECTION_STRING
```

The `Telemetry:Publisher` must be set to: Azure

You may also use the format from above to override any settings in appsettings.json.

### Reader

A telemetry reader can be found in NucuCar.TestClient. You'll need a connection string that can be found in
Azure's IoT Hub Build-In Endpoints setting.

---

## Disk Telemetry

### Publisher

Publishes telemetry on the disk.

Example connection string:
`Filename=telemetry;FileExtension=csv;Separator=,;BufferSize=4096`

The `Telemetry:Publisher` must be set to: Disk

See the source code for comments on the ConnectionString.

### Reader

You will need to parse the file by yourself.

---

## Firebase Firestore Database

### Publisher

Publishes telemetry on the firestore.

The `Telemetry:Publisher` must be set to: Firestore

Example connection string:
`ProjectId=nucuhub;CollectionName=sensors-telemetry-test;Timeout=1000`

If you want to use Authentication you can do so by providing the following keys
in the connection string: WebApiEmail, WebApiPassword, WebApiKey.

### Reader

You will need use a firebase client or rest API.