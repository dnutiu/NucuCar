package dev.nuculabs.grpc;

import NucuCarSensorsProto.NucuCarSensors;
import android.util.Log;
import org.json.JSONException;
import org.json.JSONObject;

public class EnvironmentSensorData {
    @SuppressWarnings("FieldCanBeLocal")
    private final String TAG = EnvironmentSensorData.class.getName();
    private double temperature;
    private double humidity;
    private double pressure;
    private double volatileOrganicCompounds;
    private NucuCarSensors.SensorStateEnum sensorState;

    public EnvironmentSensorData(String data, NucuCarSensors.SensorStateEnum state) {
        try {
            JSONObject json = new JSONObject(data);
            temperature = json.getDouble("Temperature");
            humidity = json.getDouble("Humidity");
            pressure = json.getDouble("Pressure");
            volatileOrganicCompounds = json.getDouble("VolatileOrganicCompounds");
            sensorState = state;
        } catch (JSONException e) {
            Log.e(TAG, e.getLocalizedMessage());
            temperature = -1;
            humidity = -1;
            pressure = -1;
            volatileOrganicCompounds = -1;
            sensorState = NucuCarSensors.SensorStateEnum.Error;
        }
    }

    public NucuCarSensors.SensorStateEnum getSensorState() {
        return sensorState;
    }

    public double getTemperature() {
        return temperature;
    }

    public double getHumidity() {
        return humidity;
    }

    public double getPressure() {
        return pressure;
    }

    public double getVolatileOrganicCompounds() {
        return volatileOrganicCompounds;
    }
}
