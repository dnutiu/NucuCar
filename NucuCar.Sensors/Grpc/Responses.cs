using NucuCarSensorsProto;

namespace NucuCar.Sensors.Grpc
{
    public static class Responses
    {
        public static readonly NucuCarSensorResponse GrpcIsDisabledResponse = new NucuCarSensorResponse()
        {
            State = SensorStateEnum.Disabled,
            JsonData = "{}"
        };
    }
}