using NucuCarSensorsProto;

namespace NucuCar.Sensors.Grpc
{
    public static class Responses
    {
        public static NucuCarSensorResponse GrpcIsDisabledResponse = new NucuCarSensorResponse()
        {
            State = SensorStateEnum.GrpcDisabled,
            JsonData = "{}"
        };
    }
}