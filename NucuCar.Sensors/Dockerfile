# To build this image use the following command:
# docker build . -f ./NucuCar/NucuCar.Sensors/Dockerfile -t NucuCar_Sensors
# To debug: docker run -it NucuCar_Sensors /bin/bash

# STAGE 0: Build the program.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1
MAINTAINER Denis Nutiu

# Copy the NucuHub folder into the workdir
WORKDIR /root/build
COPY . .

RUN dotnet build --configuration Release -o dist

# STAGE 1: Run the program.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

# Copy the built program into workdir.
WORKDIR /root/app
COPY --from=0 /root/build/dist/ .

CMD /root/app/NucuCar.Sensors
