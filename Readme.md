# Introduction (WIP)

An attempt to create a remote weather station hub on the Raspberry Pi.

The repository consists on several modules written in C# and an Android application written in Java.

Communication between the Raspberry and the Android application is done via [gRPC](https://grpc.io/).

**The project is still undergoing development and it's not production ready.**

[![CircleCI](https://circleci.com/gh/dnutiu/NucuCar.svg?style=svg)](https://circleci.com/gh/dnutiu/NucuCar)

# Architecture

TBD

# Installing and Developing

## Requirements

To build this project you will need to install .Net Core 3.1 and Android Studio.

* https://docs.microsoft.com/en-us/dotnet/core/install/
* https://developer.android.com/studio

To run this project on the Raspberry Pi you will need dotnet core 3.1. installed.



## Building

The following script provides instructions on how to build the whole project and deploy the NucuCar.Sensors module on the Raspberry Pi.

#### Manual

```$xslt
// Build project
dotnet build --runtime linux-arm -p:PublishSingleFile=true --configuration Release

// Navigate to desired project directory, for example NucuCar.Sensors.
cd NucuCar.Sensors

// Create .tar
tar -zcvf nh.tar.gz .\bin\Release\netcoreapp3.1\linux-arm\*

// Copy onto pi (replace with your ip, user)
sftp pi@192.168.0.100
sftp> put nh.tar.gz
sftp> exit

// SSH into pi
ssh pi@192.168.0.100

// Extract the archive and cd into it
tar -zxvf nh.tar.gz
cd bin/Release/netcoreapp3.1/linux-arm/

// Grant execution permissions
chmod +x NucuCar.Sensors

// Run the program
./NucuCar.Sensors
```
---

#### Docker-Compose

To run the project you can take advantage of `docker-compsose` and run it with the following commands:

```bash
git clone https://github.com/dnutiu/NucuCar.git
cd NucuCar
docker-compose up
```
_(This is not recommended as the performance will take a significant hit.)_

## Directory Structure Overview

### NucuHub.Android

Holds the Android application. 

We are currently targeting Android API 29+.

### NucuCar.Domain

Holds common classes and interfaces that are required by the projects, 
provides all the types that are generated via protocol buffers and some utility classes.

### NucuCar.Telemetry

Holds concrete implementation for telemetry publishers and workers.
The Telemetry project can be used by other projects such as 
NucuCar.Sensors to send telemetry into the cloud.

This project is not deployed.

### NucuCar.Sensors

Manages all the sensors. For more info see the readme file located in the project directory.

This module is runnable and is deployed on the Raspberry Pi.

### NucuCar.TestClient

Command line utility to play around with the car functionality. 
You can use it to remotely read data from the sensors via gRPC methods and 
to publish and read telemetry data.

### Docs

Holds project resources and documentation.
