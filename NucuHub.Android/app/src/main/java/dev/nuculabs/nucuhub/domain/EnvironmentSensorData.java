package dev.nuculabs.nucuhub.domain;

import NucuCarSensorsProto.NucuCarSensors;
import android.util.Log;
import lombok.Getter;
import org.json.JSONException;
import org.json.JSONObject;

import java.text.DecimalFormat;

public class EnvironmentSensorData {
    @SuppressWarnings("FieldCanBeLocal")
    private final String TAG = EnvironmentSensorData.class.getName();
    @Getter private double temperature;
    @Getter private double humidity;
    @Getter private double pressure;
    @Getter private double volatileOrganicCompounds;
    private NucuCarSensors.SensorStateEnum sensorState;

    public EnvironmentSensorData(String data, NucuCarSensors.SensorStateEnum state) {
        try {
            JSONObject json = new JSONObject(data);
            DecimalFormat df = new DecimalFormat("#.#");
            temperature = json.getDouble("Temperature");
            humidity = json.getDouble("Humidity");
            pressure = Double.parseDouble(df.format(json.getDouble("Pressure")));
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

    /**
     *    Computes the air quality score. The score goes from 0 to 3.
     *    For example if the temperature value is abnormal but the rest of the values are normal, the score will be 1.
     * @return The air quality score.
     */
    public int computeAirQualityScore() {
        if (getSensorState() != NucuCarSensors.SensorStateEnum.Initialized) {
            return -1;
        }

        int airQualityScore = 0;
        // Ideal room temperature range is 18-26 celsius.
        if (temperature > 26 || temperature < 20) {
            airQualityScore += 1;
        }
        // Ideal humidity for humans is 30–70%
        if (humidity < 30 || humidity > 70) {
            airQualityScore += 1;
        }
        // Let's guess that ideal VoC resistance is > 150
        if (volatileOrganicCompounds < 150) {
            airQualityScore += 1;
        }
        return airQualityScore;
    }

    @SuppressWarnings("NullableProblems")
    @Override
    public String toString() {
        return "EnvironmentSensorData{" +
                ", temperature=" + temperature +
                ", humidity=" + humidity +
                ", pressure=" + pressure +
                ", volatileOrganicCompounds=" + volatileOrganicCompounds +
                ", sensorState=" + sensorState +
                '}';
    }
}
